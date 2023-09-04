namespace HttpClientStatus;

using System;
using System.Text.RegularExpressions;

/// <summary>
/// Entry-point of Program
/// </summary>
class WordCaller
{
    /// <summary>
    /// Entry-point of WordCaller
    /// </summary>
    static void Main()
    {
        WordCounter ConsoleLogger = new();
        ConsoleLogger.MainMenu();
    }
}
/// <summary>
/// Does the actual word-counting
/// </summary>
internal class WordCounter
{
    internal List<string> _excludedList;
    internal int _numberToShow, _totalWords;
    internal Dictionary<string, int> _sortedWords;


    public WordCounter() {
        _excludedList = new List<string>();
        _numberToShow = 10;
        _sortedWords = new Dictionary<string, int>();
        _totalWords = 0;
    }


    /// <summary>
    /// Either Add or Remove an item from the Exclude List
    /// </summary>
    private void AlterExcludeList(bool isAdding)
    {
        Console.Clear();
        Console.WriteLine($"Which Word Would You Like To {(isAdding ? "Add" : "Remove")}?");
        string keyWord = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(keyWord))
        {
            return;
        }

        keyWord = CapitalizeFirstLetter(keyWord);
        if (isAdding)
        {
            if (!_excludedList.Contains(keyWord))
            {
                _excludedList.Add(keyWord);
            }
            else
            {
                Console.WriteLine("Word Already In Exclude List");
                Console.WriteLine("-----------------------");
                Console.WriteLine("Enter Any Input To Exit");
                Console.ReadLine();
            }
        }
        else
        {
            if (_excludedList.Contains(keyWord))
            {
                _excludedList.Remove(keyWord);
            }
            else
            {
                Console.WriteLine("Word Not In Exclude List");
                Console.WriteLine("-----------------------");
                Console.WriteLine("Enter Any Input To Exit");
                Console.ReadLine();
            }
        }
        return;
    }

    /// <summary>
    /// Capitalize the first letter of every word and turn alphabetic postscripts to numerals into whitespace
    /// </summary>
    private static string CapitalizeFirstLetter(string toCapitalize)
    {
        //Catch instances like 200's, 3rd, 2nd, 20th, the unlikely event that Regex missed a repeated space
        if (string.IsNullOrEmpty(toCapitalize) || toCapitalize == "s" || toCapitalize == "rd" || toCapitalize == "nd" || toCapitalize == "th"  || toCapitalize == " ")
        {
            return "";
        }
        else if (toCapitalize.Length == 1)
        {
            return toCapitalize.ToUpper();
        }
        else
        {
            return string.Concat(toCapitalize[..1].ToUpper(), toCapitalize.AsSpan(1));
        }
    }

    /// <summary>
    /// Change the display-results value to user-input
    /// </summary>
    private void ChangeNumberOfResultsToDisplay()
    {
        Console.Clear();
        Console.WriteLine("Please Enter The Number Of Words You Would Like To See Displayed");
        string choice = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(choice))
        {
            return;
        }

        if (int.TryParse(choice, out int value))
        {
            if (value < 1)
            {
                Console.WriteLine("Unable To Display Numbers Equal To Or Less Than Zero");
                Console.WriteLine("-----------------------");
                Console.WriteLine("Enter Any Input To Exit");
                Console.ReadLine();
                return;
            }
            _numberToShow = value;
            return;
        }
        Console.WriteLine("Not A Valid Number");
        Console.WriteLine("-----------------------");
        Console.WriteLine("Enter Any Input To Exit");
        Console.ReadLine();
        return;
    }

    /// <summary>
    /// Clears Exclude List
    /// </summary>
    private void ClearExcludeList()
    {
        Console.Clear();
        _excludedList= new List<string>();
        Console.WriteLine("Exclude List Has Been Cleared");
        Console.WriteLine("-----------------------");
        Console.WriteLine("Enter Any Input To Exit");
        Console.ReadLine();
    }

    /// <summary>
    /// Prints the Exclude List Line-By-Line
    /// </summary>
    private void DisplayExcludeList()
    {
        Console.Clear();
        if (_excludedList.Count == 0)
        {
            Console.WriteLine("Exclude List Is Currently Blank");
        }
        else
        {
            foreach (string exclude in _excludedList)
            {
                Console.WriteLine(exclude);
            }
        }
        Console.WriteLine("-----------------------");
        Console.WriteLine("Enter Any Input To Exit");
        Console.ReadLine();
    }

    /// <summary>
    /// Reads the Wiki-article, extracts the history-section and removes unnecessary info
    /// </summary>
    private void GetCountForWord()
    {
        Console.Clear();
        Console.WriteLine("Which Word Would You Like To Count?");
        string keyWord = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(keyWord))
        {
            return;
        }
        keyWord = CapitalizeFirstLetter(keyWord);
        if (!_sortedWords.ContainsKey(keyWord))
        {
            Console.WriteLine($"'{keyWord}' Does Not Appear In The List.");
            Console.WriteLine("-----------------------");
            Console.WriteLine("Enter Any Input To Exit");
            Console.ReadLine();
            return;
        }
        Console.WriteLine($"'{keyWord}' Appears {_sortedWords[keyWord]} Times In The List.");
        Console.WriteLine("-----------------------");
        Console.WriteLine("Enter Any Input To Exit");
        Console.ReadLine();
    }

    /// <summary>
    /// Reads the Wiki-article, extracts the history-section and removes unnecessary info
    /// </summary>
    private static async Task<string> GetHistorySection()
    {
        using var client = new HttpClient();
        var result = await client.GetStringAsync("https://en.wikipedia.org/wiki/Microsoft");
        var sections = result.Split("<h2><span class=\"mw-headline\" id=\"History\">History</span></h2>");
        string historySection = sections[1].Split("<h2><span class=\"mw-headline\" id=\"Corporate_affairs\">Corporate affairs</span></h2>")[0];
        //Remove all HTML
        string onlyWordsAndSpaces = Regex.Replace(historySection, @"<[^>]*>", "");
        //Remove anything but words and spaces
        onlyWordsAndSpaces = Regex.Replace(onlyWordsAndSpaces, @"[^a-zA-Z ]", " ");
        //Remove all repeated spaces
        onlyWordsAndSpaces = Regex.Replace(onlyWordsAndSpaces, @"\s+", " ");
        return onlyWordsAndSpaces;
    }

    /// <summary>
    /// Converts the text-article into an array of words, capitalizing each entry
    /// </summary>
    private static string[] GetTextArticleAsWordArray()
    {
        string _wikiHistoryText = GetHistorySection().Result;
        string[] articleArray = _wikiHistoryText.Split(' ');
        int totalWords = articleArray.Length;
        string[] capitalizedArticleArray = new string[totalWords];
        for (int i = 0; i < totalWords; i++)
        {
            string capitalizedWord = CapitalizeFirstLetter(articleArray[i]);
            if (!string.IsNullOrEmpty(capitalizedWord))
            {
                capitalizedArticleArray[i] = capitalizedWord;
            }
        }
        return capitalizedArticleArray;
    }

    /// <summary>
    /// Converts the Wiki-Text into a value-sorted Dictionary and displays it
    /// </summary>
    private void GetWordList()
    {
        Console.Clear();
        int totalDisplayed = 0, totalSkipped = 0;
        foreach (KeyValuePair<string, int> word in _sortedWords)
        {
            if (_excludedList.Contains(word.Key))
            {
                //opted to make the average case take longer to run rather than re-sort the entire dictionary every time the Exclude List changed
                totalSkipped++;
                continue;
            }
            totalDisplayed++;
            Console.WriteLine($"{totalDisplayed}. {word.Key}: {word.Value}");
            if (totalDisplayed >= _numberToShow)
            {
                break;
            }
        }
        if (_numberToShow > _totalWords)
        {
            Console.WriteLine($"Your Desired Display-Count Of {_numberToShow} Is Greater Than The Number Of Unique Words. Only {_totalWords - totalSkipped} Are Available.");
        }
        Console.WriteLine("-----------------------");
        Console.WriteLine("Enter Any Input To Exit");
        Console.ReadLine();
    }

    /// <summary>
    /// Shell-function for various sorting-processes
    /// </summary>
    private void HandleSorting()
    {
        string[] uncountedWords = GetTextArticleAsWordArray();
        SortedDictionary<string, int> countedWords = WriteDictionary(uncountedWords);
        SortDictionary(countedWords);
        _totalWords = _sortedWords.Count;
    }

    /// <summary>
    /// Serves as entry point into the class
    /// </summary>
    public void MainMenu()
    {
        bool keepRunning = true;
        HandleSorting();
        while (keepRunning)
        {
            Console.Clear();
            Console.WriteLine("Please Select An Option");
            Console.WriteLine("1. View Results");
            Console.WriteLine($"2. Change Number Of Words To Display: Currently: {_numberToShow}");
            Console.WriteLine("3. Add Word To The Exclude List");
            Console.WriteLine("4. Remove Word From The Exclude List");
            Console.WriteLine("5. View The Exclude List");
            Console.WriteLine("6. Clear The Exclude List");
            Console.WriteLine("7. Check Count Of Specific Word");
            Console.WriteLine("8. Quit");

            string choice = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(choice))
            {
                continue;
            }

            switch (choice)
            {
                case "1":
                    GetWordList();
                    break;
                case "2":
                    ChangeNumberOfResultsToDisplay();
                    break;
                case "3":
                    AlterExcludeList(true);
                    break;
                case "4":
                    AlterExcludeList(false);
                    break;
                case "5":
                    DisplayExcludeList();
                    break;
                case "6":
                    ClearExcludeList();
                    break;
                case "7":
                    GetCountForWord();
                    break;
                case "8":
                    keepRunning = false;
                    Console.Clear();
                    Console.WriteLine("Have A Lovely Day");
                    break;
                default:
                    Console.WriteLine("Not a Valid Input");
                    Console.WriteLine("-----------------------");
                    Console.WriteLine("Enter Any Input To Exit");
                    Console.ReadLine();
                    break;

            }
        }
    }

    /// <summary>
    /// Use an insert-sort to sort the dictionary numerically
    /// </summary>
    private void SortDictionary(SortedDictionary<string, int> nonNumericallySortedDictionary)
    {
        _sortedWords = new Dictionary<string, int>();
        int totalLength = nonNumericallySortedDictionary.Count, i, j, val, counter = 0 ;
        bool flag;
        string stringVal;
        string[] words = new string[totalLength];
        int[] counts = new int[totalLength];
        foreach(KeyValuePair<string, int> word in nonNumericallySortedDictionary )
        {
            words[counter] = word.Key;
            counts[counter] = word.Value;
            counter++;
        }
        for (i = 1; i < totalLength; i++)
        {
            val = counts[i];
            stringVal = words[i];
            flag = false;
            for ( j = i - 1; j >= 0 && !flag;)
            {
                if (val > counts[j])
                {
                    counts[j + 1] = counts[j];
                    words[j + 1] = words[j];
                    j--;
                    counts[j + 1] = val;
                    words[j + 1] = stringVal;
                }
                else flag = true;
            }
        }
        for (i = 0; i < totalLength; i++)
        {
            _sortedWords.Add(words[i], counts[i]);
        }
    }

    /// <summary>
    /// Write the initial (numerically) unsorted dictionary. 
    /// </summary>
    private static SortedDictionary<string, int> WriteDictionary(string[] uncountedWords)
    {
        SortedDictionary<string, int> countedWords = new();
        int totalUncounted = uncountedWords.Length;
        for (int i = 0; i < totalUncounted; i++)
        {
            if (string.IsNullOrEmpty(uncountedWords[i]))
            {
                continue;
            }
            if (countedWords.ContainsKey(uncountedWords[i]))
            {
                countedWords[uncountedWords[i]]++;
            }
            else
            {
                countedWords.Add(uncountedWords[i], 1);
            }
        }
        return countedWords;
    }
}
