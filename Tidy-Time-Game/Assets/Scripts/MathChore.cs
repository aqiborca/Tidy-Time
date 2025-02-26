/*
This script contains the Math Homework chore
Attached to: ???
TO DO
Make the difficulty of the math game gradual, and show questions one by one after completion
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MathChore : MonoBehaviour
{

    public TMP_Text[] questionTexts;
    public TMP_InputField[] answerInputs;
    public Button submitButton;
    public TMP_Text feedbackText;

    private List<string> questions = new List<string>();
    private List<int> correctAnswers = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateQuestions();
        submitButton.onClick.AddListener(CheckAnswers);
    }

    void GenerateQuestions()
    {
        questions.Clear();
        correctAnswers.Clear();
        string[] operators = {"+", "-"};

        for (int i = 0; i < questionTexts.Length; i++)
        {
            int num1 = Random.Range(1, 10);
            int num2 = Random.Range(1, 10);
            string op = operators[Random.Range(0, operators.Length)];

            string question = $"{num1} {op} {num2} = ";
            int answer = CalculateAnswer(num1, num2, op);

            questions.Add(question);
            correctAnswers.Add(answer);

            questionTexts[i].text = question;

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

    public void CheckAnswers()
    {
        bool allCorrect = true;
        
        for (int i = 0; i < answerInputs.Length; i++)
        {
            if (int.TryParse(answerInputs[i].text, out int userAnswer))
            {
                if (userAnswer != correctAnswers[i])
                {
                    allCorrect = false;
                }
            }
            else
            {
                allCorrect = false;
            }
        }

        if (allCorrect)
        {
            feedbackText.text = "Correct!";
            Invoke("ReturnToBedroom", 2f);
        }
        else
        {
            feedbackText.text = "Try again!";
        }
    }

    void ReturnToBedroom()
    {
        SceneManager.LoadScene("Bedroom");
    }

}
