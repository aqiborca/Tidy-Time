using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MathChore : MonoBehaviour
{
    // Question and Answer objects for 5 questions
    public TMP_Text question1Text, question2Text, question3Text, question4Text, question5Text;
    public TMP_InputField answer1Input, answer2Input, answer3Input, answer4Input, answer5Input;

    private List<string> questions = new List<string>();
    private List<int> correctAnswers = new List<int>();

    // Start is called before the first frame update
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
    }

    void AddInputValidation(TMP_InputField inputField)
    {
        // Add a listener to validate input as the user types
        inputField.onValueChanged.AddListener((value) =>
        {
            if (value.Length > 1)
            {
                // If more than one character is entered, trim it to the first character
                inputField.text = value.Substring(0, 1);
            }

            // Ensure the input is a valid digit between 1 and 9
            if (!int.TryParse(inputField.text, out int parsedValue) || parsedValue < 1 || parsedValue > 9)
            {
                inputField.text = "";
            }
        });
    }

    void GenerateQuestions()
    {
        questions.Clear();
        correctAnswers.Clear();
        string[] operators = { "+", "-" };

        for (int i = 0; i < 5; i++)  // Generates 5 questions
        {
            string op = operators[Random.Range(0, operators.Length)];
            int num1 = Random.Range(1, 10);
            int num2 = 0;

            // Ensure that num2 and num1 result in an answer between 1 and 9
            switch (op)
            {
                case "+": 
                    num2 = Random.Range(1, 10 - num1);  // Ensure the sum does not exceed 9
                    break;
                case "-": 
                    num2 = Random.Range(1, num1);  // Ensure the result is greater than or equal to 1
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
        // Display all questions at once
        question1Text.text = questions[0];
        question2Text.text = questions[1];
        question3Text.text = questions[2];
        question4Text.text = questions[3];
        question5Text.text = questions[4];

        // Clear all input fields
        answer1Input.text = "";
        answer2Input.text = "";
        answer3Input.text = "";
        answer4Input.text = "";
        answer5Input.text = "";
    }

    public void CheckAnswers()
    {
        bool allCorrect = true;

        // Check answers for each question
        if (int.TryParse(answer1Input.text, out int userAnswer1))
        {
            if (userAnswer1 < 1 || userAnswer1 > 9 || userAnswer1 != correctAnswers[0])
            {
                allCorrect = false;
                Debug.Log("Question 1: Incorrect or out of range!");
            }
            else
            {
                Debug.Log("Question 1: Correct!");
            }
        }
        else
        {
            allCorrect = false;
            Debug.Log("Question 1: Invalid input!");
        }

        if (int.TryParse(answer2Input.text, out int userAnswer2))
        {
            if (userAnswer2 < 1 || userAnswer2 > 9 || userAnswer2 != correctAnswers[1])
            {
                allCorrect = false;
                Debug.Log("Question 2: Incorrect or out of range!");
            }
            else
            {
                Debug.Log("Question 2: Correct!");
            }
        }
        else
        {
            allCorrect = false;
            Debug.Log("Question 2: Invalid input!");
        }

        if (int.TryParse(answer3Input.text, out int userAnswer3))
        {
            if (userAnswer3 < 1 || userAnswer3 > 9 || userAnswer3 != correctAnswers[2])
            {
                allCorrect = false;
                Debug.Log("Question 3: Incorrect or out of range!");
            }
            else
            {
                Debug.Log("Question 3: Correct!");
            }
        }
        else
        {
            allCorrect = false;
            Debug.Log("Question 3: Invalid input!");
        }

        if (int.TryParse(answer4Input.text, out int userAnswer4))
        {
            if (userAnswer4 < 1 || userAnswer4 > 9 || userAnswer4 != correctAnswers[3])
            {
                allCorrect = false;
                Debug.Log("Question 4: Incorrect or out of range!");
            }
            else
            {
                Debug.Log("Question 4: Correct!");
            }
        }
        else
        {
            allCorrect = false;
            Debug.Log("Question 4: Invalid input!");
        }

        if (int.TryParse(answer5Input.text, out int userAnswer5))
        {
            if (userAnswer5 < 1 || userAnswer5 > 9 || userAnswer5 != correctAnswers[4])
            {
                allCorrect = false;
                Debug.Log("Question 5: Incorrect or out of range!");
            }
            else
            {
                Debug.Log("Question 5: Correct!");
            }
        }
        else
        {
            allCorrect = false;
            Debug.Log("Question 5: Invalid input!");
        }

        if (allCorrect)
        {
            Debug.Log("All questions are correct!");
        }
        else
        {
            Debug.Log("Some questions were incorrect.");
        }
    }
}