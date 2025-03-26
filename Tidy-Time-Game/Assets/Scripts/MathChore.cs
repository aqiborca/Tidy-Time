using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MathChore : MonoBehaviour
{
    // Question and Answer objects for 5 questions
    public TMP_Text question1Text, question2Text, question3Text, question4Text, question5Text;
    public TMP_InputField answer1Input, answer2Input, answer3Input, answer4Input, answer5Input;
    
    // UI elements for individual checks
    public Button check1Button, check2Button, check3Button, check4Button, check5Button;
    
    // Color settings
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    public Color defaultColor = Color.white;
    public Color disabledColor = new Color(0.8f, 0.8f, 0.8f); // Light grey for disabled state
    [Range(0, 1)] public float highlightIntensity = 0.7f;

    // Track which questions are completed
    private bool[] completedQuestions = new bool[5];

    private List<string> questions = new List<string>();
    private List<int> correctAnswers = new List<int>();

    void Start()
    {
        GenerateQuestions();
        ShowAllQuestions();

        // Add validation listeners to input fields
        AddInputValidation(answer1Input);
        AddInputValidation(answer2Input);
        AddInputValidation(answer3Input);
        AddInputValidation(answer4Input);
        AddInputValidation(answer5Input);

        // Add listeners for individual check buttons
        check1Button.onClick.AddListener(() => CheckAnswer1());
        check2Button.onClick.AddListener(() => CheckAnswer2());
        check3Button.onClick.AddListener(() => CheckAnswer3());
        check4Button.onClick.AddListener(() => CheckAnswer4());
        check5Button.onClick.AddListener(() => CheckAnswer5());
    }

    void AddInputValidation(TMP_InputField inputField)
    {
        inputField.onValueChanged.AddListener((value) =>
        {
            if (value.Length > 0)
            {
                inputField.text = value.Substring(0, 1);
            }

            if (!int.TryParse(inputField.text, out int parsedValue) || parsedValue < 0 || parsedValue > 9)
            {
                inputField.text = "";
            }
        });
    }

    void GenerateQuestions()
    {
        questions.Clear();
        correctAnswers.Clear();
        completedQuestions = new bool[5];
        string[] operators = { "+", "-" };

        for (int i = 0; i < 5; i++)
        {
            string op = operators[Random.Range(0, operators.Length)];
            int num1 = Random.Range(1, 9);
            int num2 = 0;

            switch (op)
            {
                case "+": 
                    num2 = Random.Range(1, 9 - num1);
                    break;
                case "-": 
                    num2 = Random.Range(1, num1);
                    break;
            }

            string question = $"{num1} {op} {num2} = ";
            int answer = CalculateAnswer(num1, num2, op);

            questions.Add(question);
            correctAnswers.Add(answer);
        }
    }

    int CalculateAnswer(int num1, int num2, string op)
    {
        switch (op)
        {
            case "+": return num1 + num2;
            case "-": return num1 - num2;
            default: return 0;
        }
    }

    void ShowAllQuestions()
    {
        question1Text.text = questions[0];
        question2Text.text = questions[1];
        question3Text.text = questions[2];
        question4Text.text = questions[3];
        question5Text.text = questions[4];

        answer1Input.text = "";
        answer2Input.text = "";
        answer3Input.text = "";
        answer4Input.text = "";
        answer5Input.text = "";

        // Reset UI elements
        ResetQuestionUI(0, answer1Input, check1Button);
        ResetQuestionUI(1, answer2Input, check2Button);
        ResetQuestionUI(2, answer3Input, check3Button);
        ResetQuestionUI(3, answer4Input, check4Button);
        ResetQuestionUI(4, answer5Input, check5Button);
    }

    void ResetQuestionUI(int index, TMP_InputField inputField, Button button)
    {
        completedQuestions[index] = false;
        inputField.interactable = true;
        button.interactable = true;
        
        UpdateButtonColors(button, defaultColor);
    }

    void LockCorrectAnswer(TMP_InputField inputField, Button button)
    {
        inputField.interactable = false;
        button.interactable = false;
        
        // Set the disabled color to our correct color
        ColorBlock colors = button.colors;
        colors.disabledColor = correctColor;
        button.colors = colors;
    }

    void UpdateButtonColors(Button button, Color color)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, highlightIntensity);
        colors.pressedColor = Color.Lerp(color, Color.black, highlightIntensity);
        colors.selectedColor = color;
        button.colors = colors;
    }

    // Individual check functions
    public void CheckAnswer1()
    {
        if (completedQuestions[0]) return;
        bool isCorrect = CheckSingleAnswer(0, answer1Input);
        if (isCorrect)
        {
            LockCorrectAnswer(answer1Input, check1Button);
            completedQuestions[0] = true;
        }
        else
        {
            UpdateButtonColors(check1Button, incorrectColor);
        }
    }

    public void CheckAnswer2()
    {
        if (completedQuestions[1]) return;
        bool isCorrect = CheckSingleAnswer(1, answer2Input);
        if (isCorrect)
        {
            LockCorrectAnswer(answer2Input, check2Button);
            completedQuestions[1] = true;
        }
        else
        {
            UpdateButtonColors(check2Button, incorrectColor);
        }
    }

    public void CheckAnswer3()
    {
        if (completedQuestions[2]) return;
        bool isCorrect = CheckSingleAnswer(2, answer3Input);
        if (isCorrect)
        {
            LockCorrectAnswer(answer3Input, check3Button);
            completedQuestions[2] = true;
        }
        else
        {
            UpdateButtonColors(check3Button, incorrectColor);
        }
    }

    public void CheckAnswer4()
    {
        if (completedQuestions[3]) return;
        bool isCorrect = CheckSingleAnswer(3, answer4Input);
        if (isCorrect)
        {
            LockCorrectAnswer(answer4Input, check4Button);
            completedQuestions[3] = true;
        }
        else
        {
            UpdateButtonColors(check4Button, incorrectColor);
        }
    }

    public void CheckAnswer5()
    {
        if (completedQuestions[4]) return;
        bool isCorrect = CheckSingleAnswer(4, answer5Input);
        if (isCorrect)
        {
            LockCorrectAnswer(answer5Input, check5Button);
            completedQuestions[4] = true;
        }
        else
        {
            UpdateButtonColors(check5Button, incorrectColor);
        }
    }

    private bool CheckSingleAnswer(int questionIndex, TMP_InputField inputField)
    {
        if (int.TryParse(inputField.text, out int userAnswer))
        {
            if (userAnswer >= 0 && userAnswer <= 9 && userAnswer == correctAnswers[questionIndex])
            {
                Debug.Log($"Question {questionIndex + 1}: Correct!");
                return true;
            }
        }
        Debug.Log($"Question {questionIndex + 1}: Incorrect!");
        return false;
    }

    public void CheckAnswers()
    {
        if (!completedQuestions[0]) CheckAnswer1();
        if (!completedQuestions[1]) CheckAnswer2();
        if (!completedQuestions[2]) CheckAnswer3();
        if (!completedQuestions[3]) CheckAnswer4();
        if (!completedQuestions[4]) CheckAnswer5();
    }
}