using UnityEngine;
using System.Collections;


// An example on how to use the helper

public class MainScriptExample : MonoBehaviour {

    // Member variable to hold the instance of the helper
    private TwitchApiHelper twitchApiInstance;


    // Routine which checks for new followers periodically and handles them
    IEnumerator FollowerUpdateRoutine(string channelName)
    {
        Debug.Log("Waiting for new followers...");

        // Should add some check to stop when the program gets closed
        while (true)
        {
            // Fetch a batch of new followers
            twitchApiInstance.FetchNewFollowers(channelName, (newFollowers) => {

                // Do stuff with the new followers
                newFollowers.ForEach((follower) => {
                    Debug.Log(follower.name + " is now following!");
					GameObject.Find("_Scripts").GetComponent<ChatManager>().MessageSend(
                    	"Thanks for the follow " + follower.name + "!");
                });
            });

            yield return new WaitForSeconds(5);
        }
        //yield return null;
    }

    void Start()
    {
        // Create an instance of the helper
        twitchApiInstance = gameObject.AddComponent<TwitchApiHelper>();

        string channelName = "clossius";

        // First fetch all of the followers, this will take a moment
        twitchApiInstance.FetchAllFollowers(channelName, (list) =>
        {
            Debug.Log("Total count of followers: " + list.Count);

            // Start the routine
            StartCoroutine(FollowerUpdateRoutine(channelName));
        });
    }

    void Update()
    {

    }
}
