using SimpleJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TwitchApiHelper : MonoBehaviour
{
    private const string baseUrl = "https://api.twitch.tv/kraken/";


    // Class to hold relevant information on a follower
    // - Got to keep track of the id too. We don't want and alert for a new follow if follower changes their name
    public class Follower
    {
        public string id;
        public string name;

        // Constructor to make creating instances of this class easier
        public Follower(string _id, string _name)
        {
            id = _id;
            name = _name;
        }
    }


    // Class that's used when we want to traverse over all followers
    // - Twitch API will at the best give 100 followers per request, so we need to use the cursor to tell
    //   it on each request that we want to get a new batch of the remaining followers
    // - Cursor is a number(here just a string for the ease of use), which Twitch API gives with the followers
    private class FollowersResult
    {
        public List<Follower> followers = new List<Follower>();
        public string cursor = null;
    }


    // This member variable will keep track of all followers the helper has seen
    static private Dictionary<string, List<Follower>> oldFollowers = new Dictionary<string, List<Follower>>();


    // Private helper for separating new followers from the old ones
    private List<Follower> FindNewFollowers(string channelName, List<Follower> followerBatch)
    {
        List<Follower> newFollowers = new List<Follower>();
        followerBatch.ForEach((follower) =>
        {

            bool found = false;
            oldFollowers[channelName].ForEach((oldFollower) =>
            {
                if (oldFollower.id == follower.id)
                {
                    found = true;
                }
            });

            if (!found)
            {
                Debug.Log("couldn't find follower with id " + follower.id);
                newFollowers.Add(follower);
            }
        });
        return newFollowers;
    }


    // Private helper which parses the JSON and reads all relevant data from the results
    private FollowersResult ParseFollowersFromData(string data)
    {
        FollowersResult result = new FollowersResult();

        JSONNode root = JSON.Parse(data);

        // Save the cursor
        result.cursor = root["_cursor"];

        // Loop over the followers and add them to the list
        JSONArray follows = root["follows"].AsArray;
        for (int i = 0; i < follows.Count; i++)
        {
            string id = follows[i]["user"]["_id"];
            string name = follows[i]["user"]["display_name"];
            result.followers.Add(new Follower(id, name));
        }

        return result;
    }


    // Helper that does the dirt work for fetching new followers
    private IEnumerator FetchNewFollowersImp(string channelName, System.Action<List<Follower>> updateCallback, int limit)
    {
        string url = baseUrl + "/channels/" + channelName + "/follows?limit=" + limit;

        WWW www = new WWW(url);
        yield return www;

        // Parse the data and find new followers
        FollowersResult result = ParseFollowersFromData(www.text);
        List<Follower> newFollowers = FindNewFollowers(channelName, result.followers);

        // Add new followers in to the oldFollowers list
        newFollowers.ForEach((name) => oldFollowers[channelName].Add(name));

        // Call the callback
        updateCallback(newFollowers);
    }


    // Helper that does the dirty work of fetching all followers
    private IEnumerator FetchAllFollowersImp(string channelName, System.Action<List<Follower>> callback, List<Follower> gatheredFollowers, string cursor)
    {
        string url = baseUrl + "/channels/" + channelName + "/follows?limit=" + 100;
        if (cursor != null)
        {
            url += "&cursor=" + cursor;
        }

        WWW www = new WWW(url);
        yield return www;

        // Parse the data and find new followers
        FollowersResult result = ParseFollowersFromData(www.text);

        // Add all followers in to the gatheredFollowers list
        result.followers.ForEach((follower) => gatheredFollowers.Add(follower));

        Debug.Log("Fetched a batch of " + result.followers.Count + " follower(s)");

        // If newFollowers count isn't 100, we have reached the end
        if (result.followers.Count < 100)
        {
            // Save the followers
            oldFollowers[channelName].AddRange(gatheredFollowers);

            // Call the final callback
            callback(gatheredFollowers);
        }
        // Otherwise we still have to fetch more followers
        else
        {
            // Start a new coroutine recursively, passing the cursor and all the gathered followers to it
            StartCoroutine(FetchAllFollowersImp(channelName, callback, gatheredFollowers, result.cursor));
        }
    }


    // Public method that wraps the coroutine start and creates follower list for the channel if necessary
    public void FetchNewFollowers(string channelName, System.Action<List<Follower>> updateCallback, int limit = 25)
    {
        if (!oldFollowers.ContainsKey(channelName))
        {
            oldFollowers.Add(channelName, new List<Follower>());
        }

        StartCoroutine(FetchNewFollowersImp(channelName, updateCallback, limit));
    }


    // Pretty much the same as the above one, just fetches all the followers
    public void FetchAllFollowers(string channelName, System.Action<List<Follower>> callback)
    {
        if (!oldFollowers.ContainsKey(channelName))
        {
            oldFollowers.Add(channelName, new List<Follower>());
        }

        StartCoroutine(FetchAllFollowersImp(channelName, callback, new List<Follower>(), null));
    }

}
