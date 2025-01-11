using UnityEngine;
using UnityEngine.Networking;  // For Unity Web Requests
using UnityEngine.UI;  // For UI Text
using System.Collections;

public class CharacterLocationTracker : MonoBehaviour
{
    [Header("Location Data")]
    public float latitude;
    public float longitude;

    [Header("Google Maps API")]
    public string apiKey = "YOUR_GOOGLE_API_KEY";  // Replace with your Google API Key

    [Header("Character Controls")]
    public bool isDragging = false;
    private Vector3 lastPosition;

    [Header("UI Elements")]
    public GameObject tempTextPrefab;  // Reference to the temporary text prefab
    public float tempTextDuration = 3f;  // Duration for temporary text to stay visible

    private void OnMouseDown()
    {
        // Start dragging
        isDragging = true;
        lastPosition = transform.position;
    }

    private void OnMouseUp()
    {
        // Stop dragging
        isDragging = false;
    }

    private void Update()
    {
        // Check if we're dragging the object
        if (isDragging)
        {
            Vector3 currentPosition = transform.position;
            Vector3 delta = currentPosition - lastPosition;

            // Update the latitude/longitude based on drag direction
            latitude += delta.x * 0.0001f;  // Adjust scaling as needed
            longitude += delta.z * 0.0001f;  // Adjust scaling as needed

            lastPosition = currentPosition;

            // Display the temporary text on the Canvas
            ShowTemporaryText($"Latitude: {latitude:F6}\nLongitude: {longitude:F6}");

            // Optionally, fetch the address from the Google API
            StartCoroutine(GetAddressFromCoordinates(latitude, longitude));
        }
    }

    private void ShowTemporaryText(string message)
    {
        // Instantiate the temporary text from the prefab
        GameObject tempTextObject = Instantiate(tempTextPrefab, transform.position, Quaternion.identity);

        // Set the text of the temporary UI Text
        Text tempText = tempTextObject.GetComponent<Text>();
        tempText.text = message;

        // Make sure the text is placed on the Canvas, not the world
        tempTextObject.transform.SetParent(GameObject.Find("Canvas").transform, false);

        // Destroy the temporary text object after a certain time
        Destroy(tempTextObject, tempTextDuration);
    }

    private IEnumerator GetAddressFromCoordinates(float lat, float lon)
    {
        // Google Maps Geocoding API endpoint
        string url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={lat},{lon}&key={apiKey}";

        // Send web request
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse response
            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"Geocoding Response: {jsonResponse}");

            // Optionally, you can deserialize the JSON response to extract address details
            // For now, we're just logging the full JSON response.
        }
        else
        {
            Debug.LogError($"Error getting address: {request.error}");
        }
    }
}
