# Sync Client Usage Guide

The sync client allows your apps to save data locally for offline usage, and sync the data to the cloud database when possible.

To use the FeedHenry .NET Sync client, you should first setup the project to use the FeedHenry .NET SDK as described in [ReadMe](ReadMe.md). Then you can follow the instructions below:

## Create the sync data models

The sync data models represent the data needs to be synchronised. They should implement the [IFHSyncModel](FHSDK/Sync/IFHSyncModel.cs) interface. 

For example, the followung code creats a TaskModel that should be synced to the cloud:

````cs
public class TaskModel: IFHSyncModel
{
    public TaskModel()
    {
    }
    
    //NOTE: this field should be non-serializable
    [JsonIgnore]
    public String UID { set; get; }


    [JsonProperty("taskName")]
    public String TaksName { set; get; }


    [JsonProperty("completed")]
    public bool Completed { set ; get; }

    public override string ToString()
    {
        return string.Format("[TaskModel: UID={0}, TaksName={1}, Completed={2}]", UID, TaksName, Completed);
    }
}
````

## Get the FHSyncClient instance

Next, you need to configure the instance of the FHSyncClient. You can create a new instance of [FHSyncConfig](FHSDK/Sync/FHSyncConfig) to configure the sync client:

````cs

//create the sync config instance
FHSyncConfig syncConfig = new FHSyncConfig();
syncConfig.SyncActive = false;
syncConfig.SyncFrequency = 60;
syncConfig.AutoSyncLocalUpdates = true;
syncConfig.SyncCloud = FHSyncConfig.SyncCloudType.MBBAS;

//Get the sync client instance and initialise it with the configuration
FHSyncClient syncClient = FHSyncClient.GetInstance();
syncClient.Initialise(syncConfig);

//You can also register various events listeners on the sync client
syncClient.SyncStarted += (object sender, FHSyncNotificationEventArgs e) => {
    //do something on sync started
};

syncClient.SyncCompleted += (object sender, FHSyncNotificationEventArgs e) => {
    //dp something on sync completed
};
````

## Let the FHSyncClient manage the sync data models

Next, all you need to do is to let the sync client to manage the sync data model like this:

````cs
//let the sync client manage TaskModel
syncClient.Manage<TaskModel>("tasks", null, null);

//use the sycn client for CRUD operations

//create a new task
TaskModel task = new TaskModel()
{
    TaksName = "test",
    Completed = false
};

task = syncClient.Create<TaskModel>("tasks", task);

//read a task
TaskModel readTask = syncClient.Read<TaskModel>("tasks", task.UID);

//List tasks
List<TaskModel> tasks = syncClient.List<TaskModel>("tasks");

//Update a task
readTask.Completed = true;
readTask = syncClient.Update<TaskModel>("tasks", readTask);

//Delete a task
syncClient.Delete<TaskModel>("tasks", readTask.UID);
````

The singleton instance of FHSyncClient can be used manage multiple sync data models. 

All the changes will be saved to local file storage immediately, and sync with the cloud when possible.


