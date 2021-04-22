using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using System.Threading.Tasks;

public class VoiceControl : MonoBehaviour
{
    public GameObject allyObject;
    public AllyScript allyScript;
    GameControl gameControl;

    Text commandsText;

    public bool waitForKillConfirmation = false;
    public bool waitForTargetSpecification = false;

    List<int> matchList = new List<int>();
    Dictionary<string, Action.actionType> actionsDict;
    static string[] actionKeywords = { "left", "right", "closer", "further", "near" };

    public class Action
    {
        public enum actionType{
            TellAboutEnemies,
            TellIfEnemyDown,
            TellWhereToGoWhatToDo,
            DetermineEnemyToKill,
            ConfirmTarget,
            KillEnemyOrder,
            ForceMissionFour
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpActions();

        /*TestRecognitionLevenshteinDistance("What do I need to do");
        TestRecognitionMatchingKeywordsPercentage("What do I need to do");
        TestRecognitionLevenshteinDistance("What should I do");
        TestRecognitionMatchingKeywordsPercentage("What should I do");
        TestRecognitionLevenshteinDistance("I like enemies");
        TestRecognitionMatchingKeywordsPercentage("I like enemies");
        TestRecognitionLevenshteinDistance("Kill the enemy that is closer to me");
        TestRecognitionMatchingKeywordsPercentage("Kill the enemy that is closer to me");
        TestRecognitionLevenshteinDistance("Kill the enemy that is on my left side");
        TestRecognitionMatchingKeywordsPercentage("Kill the enemy that is on my left side");*/

        allyObject = GameObject.FindGameObjectWithTag("Ally");
        allyScript = allyObject.GetComponent<AllyScript>();
        gameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControl>();
        commandsText = GameObject.Find("CommandsText").GetComponent<Text>();

        if (gameControl.speechRecognition)
            DisplayPossibleCommands(new string[] { "Where do I go", "Where is the checkpoint", "What do I do" });
        else
            DisplayPossibleCommands(new string[] { "Press [1]: Ask where are the enemies.", "Press [2]: Ask what to do", "Press [3]: Confirm enemy status", "Press [4]: Roger" });
    }

    public void TestRecognitionLevenshteinDistance(string textToCheck)
    {
        foreach (string key in actionsDict.Keys)
        {
            matchList.Add(LevenshteinDistance(key, textToCheck));
        }

        int pos = matchList.IndexOf(matchList.Min());
        string actionText = actionsDict.ElementAt(pos).Key;
        Debug.Log($"LevenshteinDistance: textToCheck: {textToCheck} ({textToCheck.Length}). Recognized {actionText} ({actionText.Length})." +
            $"with matchList.Min() equal to {matchList.Min()}. Matched in {(actionText.Length - matchList.Min()) * 100 / actionText.Length}%");


        matchList.Clear();
    }

    public void TestRecognitionMatchingKeywordsPercentage(string textToCheck)
    {
        foreach (string key in actionsDict.Keys)
        {
            matchList.Add(MatchingKeywordsPercentage(key, textToCheck));
        }

        int pos = matchList.IndexOf(matchList.Max());
        string actionText = actionsDict.ElementAt(pos).Key;
        Debug.Log($"MatchingKeywords: textToCheck: {textToCheck} ({textToCheck.Split().Length}). Recognized {actionText} ({actionText.Split().Length})." +
            $"with matchList.Max() equal to {matchList.Max()}. Matched in {matchList.Max()}% ");

        matchList.Clear();
    }

    void SetUpActions()
    {
        actionsDict = new Dictionary<string, Action.actionType>();

        // TellAboutEnemies();
        string[] enemyNames = { "target", "tango", "enemy", "soldier" };
        for (int i = 0; i < enemyNames.Length; i++)
        {
            string pluralName = enemyNames[i] == "enemy" ? "enemies" : enemyNames[i] + "s";
            actionsDict.Add($"Where is {enemyNames[i]}", Action.actionType.TellAboutEnemies);
            actionsDict.Add($"Where is the {enemyNames[i]}", Action.actionType.TellAboutEnemies);
            actionsDict.Add($"Where are {pluralName}", Action.actionType.TellAboutEnemies);
            actionsDict.Add($"Where are the {pluralName}", Action.actionType.TellAboutEnemies);
            actionsDict.Add($"Are there any {pluralName}", Action.actionType.TellAboutEnemies);
            actionsDict.Add($"Are there any {pluralName} nearby", Action.actionType.TellAboutEnemies);
            actionsDict.Add($"Can you see any {pluralName}", Action.actionType.TellAboutEnemies);
            actionsDict.Add($"Positions of {pluralName}", Action.actionType.TellAboutEnemies);
            actionsDict.Add($"What are the positions of {pluralName}", Action.actionType.TellAboutEnemies);
            actionsDict.Add($"Do you see any {pluralName}", Action.actionType.TellAboutEnemies);
            actionsDict.Add($"How many {pluralName} do you see", Action.actionType.TellAboutEnemies);
        }

        // TellIfEnemyDown();
        actionsDict.Add("", Action.actionType.TellIfEnemyDown);
        for (int i = 0; i < enemyNames.Length; i++)
        {
            string pluralName = enemyNames[i] == "enemy" ? "enemies" : enemyNames[i] + "s";
            actionsDict.Add($"Confirm {enemyNames[i]} is killed", Action.actionType.TellIfEnemyDown);
            actionsDict.Add($"Confirm {enemyNames[i]} is down", Action.actionType.TellIfEnemyDown);
            actionsDict.Add($"Confirm {enemyNames[i]} is dead", Action.actionType.TellIfEnemyDown);
            actionsDict.Add($"Confirm {enemyNames[i]} status", Action.actionType.TellIfEnemyDown);
            actionsDict.Add($"Is {enemyNames[i]} dead", Action.actionType.TellIfEnemyDown);
            actionsDict.Add($"Is {enemyNames[i]} down", Action.actionType.TellIfEnemyDown);
            actionsDict.Add($"Is {enemyNames[i]} alive", Action.actionType.TellIfEnemyDown);
            actionsDict.Add($"Are {pluralName} dead", Action.actionType.TellIfEnemyDown);
            actionsDict.Add($"Are {pluralName} down", Action.actionType.TellIfEnemyDown);
            actionsDict.Add($"Are {pluralName} alive", Action.actionType.TellIfEnemyDown);
        }

        // TellWhereToGoWhatToDo();
        actionsDict.Add("Where do I go", Action.actionType.TellWhereToGoWhatToDo);
        actionsDict.Add("Where do I need to go", Action.actionType.TellWhereToGoWhatToDo);
        actionsDict.Add("What do I do", Action.actionType.TellWhereToGoWhatToDo);
        actionsDict.Add("What do I need to do", Action.actionType.TellWhereToGoWhatToDo);
        actionsDict.Add("What to do", Action.actionType.TellWhereToGoWhatToDo);
        actionsDict.Add("Where is checkpoint", Action.actionType.TellWhereToGoWhatToDo);
        actionsDict.Add("Where is the checkpoint", Action.actionType.TellWhereToGoWhatToDo);
        actionsDict.Add("What is mission", Action.actionType.TellWhereToGoWhatToDo);
        actionsDict.Add("What is the mission", Action.actionType.TellWhereToGoWhatToDo);

        // DetermineEnemyToKill();
        for (int i = 0; i < enemyNames.Length; i++)
        {
            actionsDict.Add($"Kill the {enemyNames[i]} on left", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Kill the {enemyNames[i]} on my left", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Kill the {enemyNames[i]} on right", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Kill the {enemyNames[i]} on my right", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Kill the {enemyNames[i]} closer", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Kill the {enemyNames[i]} closer to me", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Kill the {enemyNames[i]} further", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Kill the {enemyNames[i]} further to me", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Kill the {enemyNames[i]} near me", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"I kill the {enemyNames[i]} on left", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"I kill the {enemyNames[i]} on my left", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"I kill the {enemyNames[i]} on right", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"I kill the {enemyNames[i]} on my right", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"I kill the {enemyNames[i]} closer", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"I kill the {enemyNames[i]} closer to me", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"I kill the {enemyNames[i]} further", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"I kill the {enemyNames[i]} further to me", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"I kill the {enemyNames[i]} near me", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Left {enemyNames[i]} is yours", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Right {enemyNames[i]} is yours", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"{enemyNames[i]} on the left is yours", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"{enemyNames[i]} on the right is yours", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Closer {enemyNames[i]} is yours", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Further {enemyNames[i]} is yours", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"{enemyNames[i]} closer to me is yours", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"{enemyNames[i]} further to me is yours", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Left {enemyNames[i]} is mine", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Right {enemyNames[i]} is mine", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"{enemyNames[i]} on the left is mine", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"{enemyNames[i]} on the right is mine", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Closer {enemyNames[i]} to me is mine", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"Further {enemyNames[i]} to me is mine", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"{enemyNames[i]} closer to me is mine", Action.actionType.DetermineEnemyToKill);
            actionsDict.Add($"{enemyNames[i]} further to me is mine", Action.actionType.DetermineEnemyToKill);
        }

        // ConfirmTarget();
        actionsDict.Add($"Who you aim at", Action.actionType.ConfirmTarget);
        for (int i = 0; i < enemyNames.Length; i++)
        {
            actionsDict.Add($"Confirm {enemyNames[i]}", Action.actionType.ConfirmTarget);
            actionsDict.Add($"Confirm {enemyNames[i]} to kill", Action.actionType.ConfirmTarget);
            actionsDict.Add($"Confirm {enemyNames[i]} to shoot", Action.actionType.ConfirmTarget);
            actionsDict.Add($"Confirm your {enemyNames[i]}", Action.actionType.ConfirmTarget);
            actionsDict.Add($"Which {enemyNames[i]} is your", Action.actionType.ConfirmTarget);
            actionsDict.Add($"Which {enemyNames[i]} is yours", Action.actionType.ConfirmTarget);
            actionsDict.Add($"What is your {enemyNames[i]}", Action.actionType.ConfirmTarget);
            actionsDict.Add($"Which {enemyNames[i]} you kill", Action.actionType.ConfirmTarget);
            actionsDict.Add($"Which {enemyNames[i]} you shoot", Action.actionType.ConfirmTarget);
        }

        // KillEnemyOrder();
        actionsDict.Add("Shoot", Action.actionType.KillEnemyOrder);
        actionsDict.Add("Kill", Action.actionType.KillEnemyOrder);
        actionsDict.Add("Shoot to kill", Action.actionType.KillEnemyOrder);
        actionsDict.Add("Now", Action.actionType.KillEnemyOrder);
        actionsDict.Add("Fire", Action.actionType.KillEnemyOrder);
        for (int i = 0; i < enemyNames.Length; i++)
        {
            actionsDict.Add($"Kill {enemyNames[i]}", Action.actionType.KillEnemyOrder);
            actionsDict.Add($"Shoot {enemyNames[i]}", Action.actionType.KillEnemyOrder);
        }

        // ForceMissionFour();
        actionsDict.Add("Campfire first", Action.actionType.ForceMissionFour);
        actionsDict.Add("Clear campfire", Action.actionType.ForceMissionFour);
        for (int i = 0; i < enemyNames.Length; i++)
        {
            string pluralName = enemyNames[i] == "enemy" ? "enemies" : enemyNames[i] + "s";
            actionsDict.Add($"Deal with {pluralName} near campfire", Action.actionType.ForceMissionFour);
            actionsDict.Add($"Deal with {enemyNames[i]} near campfire", Action.actionType.ForceMissionFour);
            actionsDict.Add($"Kill {pluralName} near campfire", Action.actionType.ForceMissionFour);
            actionsDict.Add($"Kill {enemyNames[i]} near campfire", Action.actionType.ForceMissionFour);
        }
    }

    public static int MatchingKeywordsPercentage(string source, string target)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return 0;

        string[] sourceWords = source.Split();
        string[] targetWords = target.Split();

        int sLen = sourceWords.Length;

        int matchingKeywords = 0;

        for(int i = 0; i < sourceWords.Length; i++)
        {
            for(int x = 0; x < targetWords.Length; x++)
            {
                if (sourceWords[i].ToLower() == targetWords[x].ToLower())
                {
                    matchingKeywords++;
                    break;
                }
            }
        }

        return (int)(matchingKeywords * 100 / sLen);
    }

    public static int LevenshteinDistance(string source, string target)
    {
        source = source.ToLower();
        target = target.ToLower();

        if(string.IsNullOrEmpty(source))
        {
            if (string.IsNullOrEmpty(target))
                return 0;
            else
                return target.Length;
        }

        if (string.IsNullOrEmpty(target))
            return source.Length;

        if(source.Length > target.Length)
        {
            string temp = target;
            target = source;
            source = temp;
        }

        var m = target.Length;
        var n = source.Length;
        var distance = new int[2, m + 1];

        // Initializing the distance 'matrix'
        for(var j = 1; j <= m; j++)
        {
            distance[0, j] = j;
        }

        var currRow = 0;
        for(var i = 1; i <= n; ++i)
        {
            currRow = i & 1;
            distance[currRow, 0] = i;
            var prevRow = currRow ^ 1;
            for(var j = 1; j <= m; j++)
            {
                var cost = (target[j - 1] == source[i - 1] ? 0 : 1);
                distance[currRow, j] = Mathf.Min(Mathf.Min(
                    distance[prevRow, j] + 1,
                    distance[currRow, j - 1] + 1),
                    distance[prevRow, j - 1] + cost);
            }
        }

        return distance[currRow, m];
    }

    public void CheckKeywordsAndFireEvents(string recognizedString)
    {
        Debug.Log("CheckKeywordsAndFireEvents(" + recognizedString + ")");
        //allyScript.TellAboutEnemies();
        string textToCheck = recognizedString.ToLower();
        textToCheck = textToCheck.Replace(",", "");
        textToCheck = textToCheck.Replace(".", "");
        textToCheck = textToCheck.Replace("?", "");
        textToCheck = textToCheck.Replace("!", "");
        textToCheck = textToCheck.Replace("Charlie", "");

        // Checking will be done word by word
        string[] expressions = textToCheck.Split(' ');

        bool stopNextAction = false;

        // Checking if the action is to be stopped
        foreach (string expression in expressions)
        {
            string compare = expression.Trim();
            if (stopNextAction == false)
            {
                if (compare.Contains("no") && compare.Length == 2)
                {
                    stopNextAction = true;
                }
                else if (compare.Contains("not") && compare.Length == 3)
                {
                    stopNextAction = true;
                }
                else if (compare.Contains("do not"))
                {
                    stopNextAction = true;
                }
                else if (compare.Contains("don't"))
                {
                    stopNextAction = true;
                }
            }
        }

        if (stopNextAction)
            return;

        int currMission = gameControl.GetCurrentMission();
        bool foundMatch = false;
        string matchText = "";
        string keyword = "";
        int matchPos = 0;

        if (textToCheck.Contains("roger") || textToCheck.Contains("understood") || textToCheck == "got it" || textToCheck == "thanks")
        {
            allyScript.rogerThat = true;
            allyScript.rogerThatText = textToCheck.Substring(0, 1).ToUpper() + textToCheck.Substring(1);
            return;
        }

        matchList.Clear();

        // Check what type of speech is expected depending on current gameplay status
        if (waitForTargetSpecification)
        {
            bool foundKeyword = false;
            // Looking only for determining the target
            // If current mission is 2 or 4 - action keywords must be present
            if (currMission == 2 || currMission == 4)
            {
                for(int i = 0; i < actionKeywords.Length; i++)
                {
                    if(textToCheck.Contains(actionKeywords[i]))
                    {
                        foundKeyword = true;
                        keyword = actionKeywords[i];
                        break;
                    }
                }

                if(foundKeyword)
                {
                    foreach (string key in actionsDict.Keys)
                    {
                        if (actionsDict[key] == Action.actionType.DetermineEnemyToKill)
                        {
                            matchList.Add(MatchingKeywordsPercentage(key, textToCheck));
                        }
                        else
                        {
                            matchList.Add(0);
                        }
                        /*if (actionsDict[key] == Action.actionType.DetermineEnemyToKill)
                        {
                            matchList.Add(LevenshteinDistance(key, textToCheck));
                        }
                        else
                        {
                            matchList.Add(int.MaxValue);
                        }*/
                    }

                    /*matchPos = matchList.IndexOf(matchList.Min());
                    matchText = actionsDict.ElementAt(matchPos).Key;
                    // Determinig the % of match
                    if (((matchText.Length - matchList.Min()) * 100) / matchText.Length > 60)
                        foundMatch = true;
                     */

                    matchPos = matchList.IndexOf(matchList.Max());
                    matchText = actionsDict.ElementAt(matchPos).Key;
                    // Determinig the % of match
                    if (matchList.Max() > 60)
                    {
                        foundMatch = true;                   
                    }
                }
                else
                {
                    // Keyword is not specified so looking for other possible text actions
                    foreach (string key in actionsDict.Keys)
                    {
                        // Looking only tell what to do, tell about the enemies, tell if enemy down
                        if (actionsDict[key] == Action.actionType.TellWhereToGoWhatToDo || actionsDict[key] == Action.actionType.TellAboutEnemies ||
                            actionsDict[key] == Action.actionType.TellIfEnemyDown)
                        {
                            matchList.Add(MatchingKeywordsPercentage(key, textToCheck));
                        }
                        else
                        {
                            matchList.Add(0);
                        }

                        /*if (actionsDict[key] == Action.actionType.TellWhereToGoWhatToDo || actionsDict[key] == Action.actionType.TellAboutEnemies ||
                            actionsDict[key] == Action.actionType.TellIfEnemyDown)
                        {
                            matchList.Add(LevenshteinDistance(key, textToCheck));
                        }
                        else
                        {
                            matchList.Add(int.MaxValue);
                        }*/
                    }

                    /*matchPos = matchList.IndexOf(matchList.Min());
                    matchText = actionsDict.ElementAt(matchPos).Key;
                    // Determinig the % of match
                    if (((matchText.Length - matchList.Min()) * 100) / matchText.Length > 60)
                        CallAllyAction(actionsDict.ElementAt(matchPos).Value);*/

                    matchPos = matchList.IndexOf(matchList.Max());
                    // Determinig the % of match
                    if (matchList.Max() > 60)
                    {
                        CallAllyAction(actionsDict.ElementAt(matchPos).Value);
                    }
                }
            }

            // If successfully found a possible match
            if(foundKeyword && foundMatch)
            {
                bool determineBasedOnPlayerDecision = false;
                string[] split = matchText.Split();
                
                foreach(string word in split)
                {
                    if(word == "I" || word == "mine")
                    {
                        determineBasedOnPlayerDecision = true;
                        break;
                    }
                }

                if(determineBasedOnPlayerDecision)
                {
                    // Find the opposite word
                    string temp = "";
                    switch (keyword)
                    {
                        case "left":
                            temp = "right";
                            break;
                        case "right":
                            temp = "left";
                            break;
                        case "further":
                            temp = "closer";
                            break;
                        case "near":
                            temp = "closer";
                            break;
                        case "closer":
                            temp = "further";
                            break;
                        default:
                            temp = "left";
                            break;
                    }

                    allyScript.determineEnemyToKillPos = temp;
                    allyScript.determineEnemyToKill = true;

                    /*if(allyScript.DetermineEnemyToKill(temp))
                    {
                        waitForTargetSpecification = false;
                        waitForKillConfirmation = true;
                    }*/
                }
                else
                {
                    allyScript.determineEnemyToKillPos = keyword;
                    allyScript.determineEnemyToKill = true;
                    
                    /*if (allyScript.DetermineEnemyToKill(keyword))
                    {
                        waitForTargetSpecification = false;
                        waitForKillConfirmation = true;
                    }*/
                }
            }
            // Else if the keyword was used but no match was found.
            else if(foundKeyword)
            {
                allyScript.notUnderstood = true;
            }
        }
        else if(waitForKillConfirmation)
        {
            // Looking only for kill confirmation, enemy kill order, tell me what to do, tell about enemies, tell if enemy down
            foreach (string key in actionsDict.Keys)
            {
                // Looking only tell what to do, tell about the enemies, tell if enemy down
                if (actionsDict[key] == Action.actionType.ConfirmTarget || actionsDict[key] == Action.actionType.KillEnemyOrder ||
                    actionsDict[key] == Action.actionType.TellWhereToGoWhatToDo || actionsDict[key] == Action.actionType.TellAboutEnemies ||
                    actionsDict[key] == Action.actionType.TellIfEnemyDown)
                {
                    matchList.Add(MatchingKeywordsPercentage(key, textToCheck));
                }
                else
                {
                    matchList.Add(0);
                }

                /*if (actionsDict[key] == Action.actionType.ConfirmTarget || actionsDict[key] == Action.actionType.KillEnemyOrder ||
                    actionsDict[key] == Action.actionType.TellWhereToGoWhatToDo || actionsDict[key] == Action.actionType.TellAboutEnemies ||
                    actionsDict[key] == Action.actionType.TellIfEnemyDown)
                {
                    matchList.Add(LevenshteinDistance(key, textToCheck));
                }
                else
                {
                    matchList.Add(int.MaxValue);
                }*/
            }

            /*matchPos = matchList.IndexOf(matchList.Min());
            matchText = actionsDict.ElementAt(matchPos).Key;
            // Determinig the % of match
            if (((matchText.Length - matchList.Min()) * 100) / matchText.Length > 60)
            {
                CallAllyAction(actionsDict.ElementAt(matchPos).Value);
                // If executed the kill order
                if (actionsDict.ElementAt(matchPos).Value == Action.actionType.KillEnemyOrder)
                    waitForKillConfirmation = false;
            }*/

            matchPos = matchList.IndexOf(matchList.Max());
            // Determinig the % of match
            if (matchList.Max() > 60)
            {
                CallAllyAction(actionsDict.ElementAt(matchPos).Value);
                // If executed the kill order
                if (actionsDict.ElementAt(matchPos).Value == Action.actionType.KillEnemyOrder)
                {
                    waitForKillConfirmation = false;
                    DisplayPossibleCommands(new string[] { "Where do I go", "Where is the checkpoint", "What do I do", "Are targets down" });
                }            
            }
        }
        else if(currMission == 3)
        {
            // Looking only for standard questions or if the player wants to deal with campfire as well
            // Deal with enemies at campfire - call AllyScript.ForceMissionFour()
            foreach (string key in actionsDict.Keys)
            {
                // Looking only tell what to do, tell about the enemies, tell if enemy down
                if (actionsDict[key] == Action.actionType.TellWhereToGoWhatToDo || actionsDict[key] == Action.actionType.TellAboutEnemies ||
                    actionsDict[key] == Action.actionType.ForceMissionFour)
                {
                    matchList.Add(MatchingKeywordsPercentage(key, textToCheck));
                }
                else
                {
                    matchList.Add(0);
                }

                /*if (actionsDict[key] == Action.actionType.TellWhereToGoWhatToDo || actionsDict[key] == Action.actionType.TellAboutEnemies ||
                    actionsDict[key] == Action.actionType.ForceMissionFour)
                {
                    matchList.Add(LevenshteinDistance(key, textToCheck));
                }
                else
                {
                    matchList.Add(int.MaxValue);
                }*/
            }

            /*matchPos = matchList.IndexOf(matchList.Min());
            matchText = actionsDict.ElementAt(matchPos).Key;
            // Determinig the % of match
            if (((matchText.Length - matchList.Min()) * 100) / matchText.Length > 60)
                CallAllyAction(actionsDict.ElementAt(matchPos).Value);*/

            matchPos = matchList.IndexOf(matchList.Max());
            // Determinig the % of match
            if (matchList.Max() > 60)
            {
                CallAllyAction(actionsDict.ElementAt(matchPos).Value);
            }
        }
        else
        {
            // Looking only for standard questions
            foreach (string key in actionsDict.Keys)
            {
                // Looking only tell what to do, tell about the enemies, tell if enemy down
                if (actionsDict[key] == Action.actionType.TellWhereToGoWhatToDo || actionsDict[key] == Action.actionType.TellAboutEnemies ||
                    actionsDict[key] == Action.actionType.TellIfEnemyDown)
                {
                    matchList.Add(MatchingKeywordsPercentage(key, textToCheck));
                }
                else
                {
                    matchList.Add(0);
                }

                /*if (actionsDict[key] == Action.actionType.TellWhereToGoWhatToDo || actionsDict[key] == Action.actionType.TellAboutEnemies)
                {
                    matchList.Add(LevenshteinDistance(key, textToCheck));
                }
                else
                {
                    matchList.Add(int.MaxValue);
                }*/
            }

            /*matchPos = matchList.IndexOf(matchList.Min());
            matchText = actionsDict.ElementAt(matchPos).Key;
            // Determinig the % of match
            if (((matchText.Length - matchList.Min()) * 100) / matchText.Length > 60)
                CallAllyAction(actionsDict.ElementAt(matchPos).Value);*/

            matchPos = matchList.IndexOf(matchList.Max());

            // Determinig the % of match
            if (matchList.Max() > 60)
            {
                Debug.Log("HERE 5");
                CallAllyAction(actionsDict.ElementAt(matchPos).Value); 
            }
        }

        Debug.Log($"Matchpos: {matchPos}, matchlist.Max {matchList.Max()}, len {matchList.Count}");

        if(matchList.Max() <= 60)
        {
            allyScript.notUnderstood = true;
        }
    }

    private void CallAllyAction(Action.actionType typeOfAction)
    {
        Debug.Log($"CallAllyAction({typeOfAction})");

        switch(typeOfAction)
        {
            case Action.actionType.ConfirmTarget:
                //allyScript.ConfirmTarget();
                allyScript.confirmTarget = true;
                break;
            /*case Action.actionType.DetermineEnemyToKill:
                allyScript.DetermineEnemyToKill(null);
                break;*/
            case Action.actionType.ForceMissionFour:
                //allyScript.ForceMissionFour();
                allyScript.forceMissionFour = true;
                break;
            case Action.actionType.KillEnemyOrder:
                //allyScript.KillEnemyOrder();
                allyScript.killEnemyOrder = true;
                break;
            case Action.actionType.TellAboutEnemies:
                //allyScript.TellAboutEnemies();
                allyScript.tellAboutEnemies = true;
                break;
            case Action.actionType.TellIfEnemyDown:
                //allyScript.TellIfEnemyDown();
                allyScript.tellIfEnemyDown = true;
                break;
            case Action.actionType.TellWhereToGoWhatToDo:
                //allyScript.TellWhereToGoWhatToDo();
                allyScript.tellWhereToGoWhatToDo = true;
                break;
            default:
                break;
        }
    }

    public void DisplayPossibleCommands(string[] commandsArray)
    {
        if (gameControl.speechRecognition)
            commandsText.text = "Try these commands [T]:\nRoger / Understood / Got it\n";
        else
            commandsText.text = "Commands:\n";

        for (int i = 0; i < commandsArray.Length; i++)
        {
            commandsText.text += commandsArray[i] + "\n";
        }
    }
}
