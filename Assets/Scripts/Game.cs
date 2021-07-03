using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    int red, blue, yellow, green, white1, white2, players, activePlayer;
    int[] dice;
    string[] diceNames = new string[] { "White: ", "White: ", "Red: ", "Yellow: ", "Green: ", "Blue: " };
    public Card card1, card2, card3, card4;
    Card[] cards;
    Dictionary<string, int> responseToInt = new Dictionary<string, int>()
        {
            { "R", 0 },
            { "Y", 1 },
            { "G", 2 },
            { "B", 3 }
        };
    Dictionary<string, string> responseToString = new Dictionary<string, string>()
        {
            { "R", "Red" },
            { "Y", "Yellow" },
            { "G", "Green" },
            { "B", "Blue" }
        };
    List<int> lockedRows = new List<int>();
    public Button bOne, bTwo, bThree, bFour;
    Button[] buttons;
    public GameObject canvasObj, t1, t2;
    public GameObject[] whiteDice1, whiteDice2, redDice, yellowDice, greenDice, blueDice, borders;
    GameObject[][] diceObj;

    // Start is called before the first frame update
    void Start()
    {
        initGame();
        addListeners();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void addListeners()
    {
        bOne.onClick.AddListener(delegate { press(1); });
        bTwo.onClick.AddListener(delegate { press(2); });
        bThree.onClick.AddListener(delegate { press(3); });
        bFour.onClick.AddListener(delegate { press(4); });
    }

    void hideCards()
    { 
        foreach(Card c in cards)
        {
            c.gameObject.SetActive(false);
        }
    }

    void showCards()
    {
        for(int i=0; i<players; i++)
        {
            cards[i].gameObject.SetActive(true);
        }
    }

    private IEnumerator hideButtons(int b)
    {
        yield return new WaitForSeconds(0.1f);
        for(int i=0; i<4; i++)
        {
            if (i == b-1) continue;
            for (int j = 0; j < 10; j++)
            {
                buttons[i].gameObject.transform.localScale += new Vector3(0.01f, 0.01f, 0f);
                yield return new WaitForSeconds(0.01f);
            }
            for (int j = 0; j < 110; j++)
            {
                buttons[i].gameObject.transform.localScale += new Vector3(-0.01f, -0.01f, 0f);
                yield return new WaitForSeconds(0.001f);
            }
        }
        float f = buttons[b - 1].gameObject.transform.localPosition.x;
        for (int j=0; j<200; j++)
        {
            buttons[b - 1].gameObject.transform.localPosition += new Vector3((0-f)/200,0,0);
            yield return new WaitForSeconds(0.00001f*Mathf.Abs(f));
        }
        for (int j = 0; j < 10; j++)
        {
            buttons[b-1].gameObject.transform.localScale += new Vector3(0.01f, 0.01f, 0f);
            yield return new WaitForSeconds(0.01f);
        }
        for (int j = 0; j < 110; j++)
        {
            buttons[b-1].gameObject.transform.localScale += new Vector3(-0.01f, -0.01f, 0f);
            yield return new WaitForSeconds(0.001f);
        }
        for (int j = 0; j < 100; j++)
        {
            t1.transform.localPosition += new Vector3(10f, 0f, 0f);
            t2.transform.localPosition += new Vector3(10f, 0f, 0f);
            yield return new WaitForSeconds(0.001f);
        }
        canvasObj.SetActive(false);
        start();
    }

    void initGame()
    {
        cards = new Card[] { card1, card2, card3, card4 };
        buttons = new Button[] { bOne, bTwo, bThree, bFour };
        diceObj = new GameObject[][] { whiteDice1, whiteDice2, redDice, yellowDice, greenDice, blueDice };
        activePlayer = 1;
        canvasObj.SetActive(true);
        hideCards();
        hideDice();
    }

    void hideDice()
    {
        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                diceObj[i][j].SetActive(false);
            }
        }
    }

    void press(int p)
    {
        players = p;
        StartCoroutine(hideButtons(p));
    }

    void start()
    {
        showCards();
        Debug.Log("Starting game with " + players + " players! (not actually)");
        roll();
    }

    void refreshBorders()
    {
        foreach(GameObject o in borders)
        {
            o.SetActive(false);
        }
        borders[activePlayer - 1].SetActive(true);
    }

    void turn()
    {
        refreshBorders();
        bool passed = false;
        int whiteSum = dice[0] + dice[1];
        List<int> pRed = new List<int>(), pYellow = new List<int>(), pGreen = new List<int>(), pBlue = new List<int>();
        pRed.Add(white1 + red);
        pRed.Add(white2 + red);
        pYellow.Add(white1 + yellow);
        pYellow.Add(white2 + yellow);
        pGreen.Add(white1 + green);
        pGreen.Add(white2 + green);
        pBlue.Add(white1 + blue);
        pBlue.Add(white2 + blue);
        List<int>[] lists = new List<int>[] { pRed, pYellow, pGreen, pBlue };
        List<string> toBeLocked = new List<string>();

        Regex rx = new Regex(@"^([GRBY])$");
        //Console.Clear();
        Debug.Log("It is player " + activePlayer + "'s turn, however, boards are marked in player order regardless of whose turn it is.");
        for (int i = 0; i < players; i++)
        {
            cards[i].updateCard();
            printDice();
            bool yes = i + 1 == activePlayer;
            if (yes)
            {
                Debug.Log("You are the active player. In addition to marking off the sum of the white dice, you will also have the option of marking off the sum of one white die and one colored die.");
            }
        Input:
            Debug.Log("Player " + (i + 1) + ", where should the white dice sum (" + whiteSum + ") be marked off? Type 'R', 'Y', 'B', or 'G' to mark a row, or 'N' to not mark any row.");
            string response = "";//fix
            Debug.Log("You entered: " + response);
            bool p = false;
            if (response.Equals("N"))
            {
                if (!passed) { passed = yes; }
                p = true;
                Debug.Log("You chose not to mark off any row with the sum of the white dice.");
            }
            else if (rx.IsMatch(response))
            {
                if (lockedRows.Contains(responseToInt[response]))
                {
                    Debug.Log("That row has been locked!");
                    goto Input;
                }
                if (cards[i].getRows()[responseToInt[response]].addMarkedNumber(whiteSum))
                {
                    Debug.Log("The number " + whiteSum + " was marked off of the " + responseToString[response].ToLower() + " row.");
                    if (cards[i].getRows()[responseToInt[response]].getStartsSmall())
                    {
                        if (whiteSum == 12)
                        {
                            cards[i].getRows()[responseToInt[response]].addMarkedNumber(13);
                            toBeLocked.Add(response);
                        }
                    }
                    else
                    {
                        if (whiteSum == 2)
                        {
                            cards[i].getRows()[responseToInt[response]].addMarkedNumber(1);
                            toBeLocked.Add(response);
                        }
                    }
                }
                else
                {
                    Debug.Log("The number " + whiteSum + " could not be marked off of the " + responseToString[response].ToLower() + " row.");
                    goto Input;
                }
            }
            else
            {
                Debug.Log("Invalid input!");
                goto Input;
            }
            if (!p)
            {
                //Console.Clear();
                Debug.Log("After any marks you made this turn, your board now looks like this:");
                cards[i].updateCard();
            }
            Debug.Log("Press [Enter] to move to the next player.");
            //while (Console.ReadKey().Key != ConsoleKey.Enter) { }
            Thread.Sleep(100);
            //Console.Clear();
        }
        if (toBeLocked.Count > 0)
        {
            foreach (string i in toBeLocked)
            {
                lockedRows.Add(responseToInt[i]);
                Debug.Log("The " + responseToString[i].ToLower() + " row was locked!");
            }
            if (lockedRows.Count > 1)
            {
                end(2);
                return;
            }
            Debug.Log("Press [Enter] to continue to player " + activePlayer + "'s second move.");
            //while (Console.ReadKey().Key != ConsoleKey.Enter) { }
            Thread.Sleep(100);
        }
        //Console.Clear();

        bool p2 = false;
        cards[activePlayer - 1].updateCard();
        printDice();
        Debug.Log("You (player " + activePlayer + ") may now mark off an additional number from one row by adding one colored die to one white die. Please enter the row and number you'd like to mark off (as in 'R3' or 'B11', etc) or enter 'N' to not mark any row.");
        if (passed) { Debug.Log("Since you passed your last move, passing again would result in a penalty."); }
    Input2:
        string response2 = "";//fix
        int l = response2.Length;
        if (l < 4)
        {
            if (response2.Equals("N"))
            {
                p2 = true;
                if (passed)
                {
                    cards[activePlayer - 1].addPenalty();
                    int p = cards[activePlayer - 1].getPenalties();
                    Debug.Log("Since you did not mark off any numbers this turn, you receive a penalty. You now have " + p + " penalties.");
                    if (p > 3)
                    {
                        end(1);
                        return;
                    }
                }
                else
                {
                    Debug.Log("You chose not to mark off any row with the sum of a white die and a colored die.");
                }
            }
            else if (l > 1)
            {
                string letter = response2.Substring(0, 1);
                int num = 0;
                if (!int.TryParse(response2.Substring(1, l - 1), out num))
                {
                    Debug.Log("Invalid input!");
                    goto Input2;
                }
                if (!rx.IsMatch(letter) || num > 12 || num < 2)
                {
                    Debug.Log("Invalid input!");
                    goto Input2;
                }
                else
                {
                    if (lists[responseToInt[letter]].Contains(num))
                    {
                        if (lockedRows.Contains(responseToInt[letter]))
                        {
                            Debug.Log("That row has been locked!");
                            goto Input2;
                        }
                        if (cards[activePlayer - 1].getRows()[responseToInt[letter]].addMarkedNumber(num))
                        {
                            Debug.Log("The number " + num + " was marked off of the " + responseToString[letter].ToLower() + " row.");
                            if (cards[activePlayer - 1].getRows()[responseToInt[letter]].getStartsSmall())
                            {
                                if (num == 12)
                                {
                                    cards[activePlayer - 1].getRows()[responseToInt[letter]].addMarkedNumber(13);
                                    lockedRows.Add(responseToInt[letter]);
                                }
                            }
                            else
                            {
                                if (num == 2)
                                {
                                    cards[activePlayer - 1].getRows()[responseToInt[letter]].addMarkedNumber(1);
                                    lockedRows.Add(responseToInt[letter]);
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("The number " + num + " could not be marked off of the " + responseToString[letter].ToLower() + " row.");
                            goto Input2;
                        }
                    }
                    else
                    {
                        Debug.Log("The " + responseToString[letter].ToLower() + " die (" + dice[responseToInt[letter]] + ") can not be added to either of the white dice (" + white1 + " or " + white2 + ") to make a " + num + ".");
                        goto Input2;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Invalid input!");
            goto Input2;
        }
        if (!p2)
        {
            //Console.Clear();
            Debug.Log("After any marks you made this turn, your board now looks like this:");
            cards[activePlayer - 1].updateCard();
        }
        Debug.Log("Press [Enter] to continue to the next turn.");
        //while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        //Thread.Sleep(100);
        //Console.Clear();
        if (lockedRows.Count > 1)
        {
            end(2);
            return;
        }
        activePlayer++;
        if (activePlayer > players)
        {
            activePlayer = 1;
        }
        turn();
    }

    void end(int how)//1 = penalties, 2 = two rows locked
    {
        //Console.Clear();
        switch (how)
        {
            case 1:
                Debug.Log("The game ended! One player got 4 penalties.");
                break;
            case 2:
                Debug.Log("The game ended! Two rows have been locked!");
                break;
            default:
                break;
        }
        int[] scores = new int[] { 0, 0, 0, 0 };
        for (int i = 0; i < players; i++)
        {
            Debug.Log("Here is Player " + (i + 1) + "'s final board and score:");
            cards[i].updateCard();
            scores[i] = cards[i].printScore();
            Debug.Log("-------------------------------------------------------------");
        }
    }

    void roll()
    {
        red = Random.Range(1, 7);
        blue = Random.Range(1, 7);
        yellow = Random.Range(1, 7);
        green = Random.Range(1, 7);
        white1 = Random.Range(1, 7);
        white2 = Random.Range(1, 7);
        dice = new int[] { white1, white2, red, yellow, green, blue };
        StartCoroutine(printDice());
    }

    private IEnumerator printDice()
    {
        for (int i = 0; i < 6; i++)
        {    
            Debug.Log(diceNames[i] + dice[i]);
            diceObj[i][dice[i] - 1].transform.localScale = new Vector3(0, 0, 0);
            diceObj[i][dice[i] - 1].SetActive(true);
            for (int j = 0; j < 10; j++)
            {
                diceObj[i][dice[i] - 1].transform.localScale += new Vector3(0.1f,0.1f,0.1f);
                yield return new WaitForSeconds(0.025f);
            }
        }


    }

}
