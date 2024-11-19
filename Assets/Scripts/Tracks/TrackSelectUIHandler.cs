using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrackSelectUIHandler : MonoBehaviour
{
    public Button course1Button;
    public Button course2Button;
    public Button course3Button; // Button for Course 3
    public Button course4Button; // Button for Course 4
    public Button course5Button; // Button for Course 5
    public Button randomCourseButton;

    private void Start()
    {
        // Assign the button click listeners
        course1Button.onClick.AddListener(() => SelectTrack("Course1"));
        course2Button.onClick.AddListener(() => SelectTrack("Course2"));
        course3Button.onClick.AddListener(() => SelectTrack("Course3"));
        course4Button.onClick.AddListener(() => SelectTrack("Course4"));
        course5Button.onClick.AddListener(() => SelectTrack("Course5")); // Added listener for Course 5
        randomCourseButton.onClick.AddListener(SelectRandomTrack);
    }

    private void SelectTrack(string courseName)
    {
        // Save the selected course in PlayerPrefs
        PlayerPrefs.SetString("SelectedCourse", courseName);
        PlayerPrefs.Save();

        // Load the selected scene
        SceneManager.LoadScene(courseName);
    }

    private void SelectRandomTrack()
    {
        // Select a random track (could be randomized based on your game logic)
        string[] tracks = { "Course1", "Course2", "Course3", "Course4", "Course5" }; // Updated to include Course5
        string randomTrack = tracks[Random.Range(0, tracks.Length)];

        // Save the selected random track
        PlayerPrefs.SetString("SelectedCourse", randomTrack);
        PlayerPrefs.Save();

        // Load the selected random track
        SceneManager.LoadScene(randomTrack);
    }
}









