using BranchInfo.Model;
using LibGit2Sharp;
using Newtonsoft.Json;
using System.IO;
using BranchInfo.Common;

namespace BranchInfo.Business;

/// <summary>
/// Provides the functions for the interaction with the branches
/// </summary>
internal class BranchManager
{
    /// <summary>
    /// Contains the path of the branch file
    /// </summary>
    private readonly string _branchFile = Path.Combine(AppContext.BaseDirectory, "BranchSettings.json");

    /// <summary>
    /// Gets the list with the branches
    /// </summary>
    public List<BranchEntry> BranchList { get; private set; } = [];

    /// <summary>
    /// Loads the list of branches and stores them into <see cref="BranchList"/>
    /// </summary>
    /// <returns>The awaitable task</returns>
    public async Task LoadBranchListAsync()
    {
        if (!File.Exists(_branchFile))
            return;

        var content = await File.ReadAllTextAsync(_branchFile);

        BranchList = JsonConvert.DeserializeObject<List<BranchEntry>>(content) ?? [];

        if (BranchList.Count == 0) 
            return;

        foreach (var branch in BranchList)
        {
            LoadBranchInformation(branch);
        }
    }

    /// <summary>
    /// Adds a new branch to the <see cref="BranchList"/>
    /// </summary>
    /// <param name="branch">The branch which should be added</param>
    /// <returns>The awaitable task</returns>
    public Task AddBranchAsync(BranchEntry branch)
    {
        branch.Id = BranchList.Count == 0 ? 1 : BranchList.Max(m => m.Id) + 1;

        LoadBranchInformation(branch); // Load the information
        BranchList.Add(branch); // Add the branch to the list

        return SaveBranchListAsync(); // Save the list
    }

    /// <summary>
    /// Removes the desired branch from the list
    /// </summary>
    /// <param name="branch">The branch which should be removed</param>
    /// <returns>The awaitable task</returns>
    public Task RemoveBranchAsync(BranchEntry branch)
    {
        BranchList.Remove(branch);

        return SaveBranchListAsync();
    }

    /// <summary>
    /// Saves the current branch list
    /// </summary>
    /// <returns>The awaitable task</returns>
    public Task SaveBranchListAsync()
    {
        var content = JsonConvert.SerializeObject(BranchList, Formatting.Indented);

        return File.WriteAllTextAsync(_branchFile, content);
    }

    /// <summary>
    /// Loads the information of the branch
    /// </summary>
    /// <param name="branch">The branch which should be checked</param>
    public static void LoadBranchInformation(BranchEntry branch)
    {
        if (!Directory.Exists(branch.GitDirectory))
        {
            branch.FriendlyName = "undefined";
            branch.LastCheck = DateTime.Now;
            return;
        }

        using var repo = new Repository(branch.GitDirectory);

        var changes = repo.Diff.Compare<TreeChanges>();
        branch.FriendlyName = repo.Head.FriendlyName;
        branch.DiffFiles = GetDiffFiles(changes);
        branch.LastCheck = DateTime.Now;
        

        // Get the last commit
        var lastCommit = repo.Commits.FirstOrDefault(); // Get the first entry (the entries are ordered descending)
        branch.LastCommit = lastCommit != null ? $"{lastCommit.Author.Name} - {lastCommit.Author.When:yyyy-MM-dd HH:mm:ss}" : "undefined";
    }

    /// <summary>
    /// Gets the diff files
    /// </summary>
    /// <param name="changes">The changes</param>
    /// <returns>The list with the diff files</returns>
    private static List<DiffFileEntry> GetDiffFiles(TreeChanges? changes)
    {
        if (changes == null)
            return [];

        var result = new List<DiffFileEntry>();

        result.AddRange(GetValues(changes.Added, DiffFileType.Added));
        result.AddRange(GetValues(changes.Deleted, DiffFileType.Delete));
        result.AddRange(GetValues(changes.Modified, DiffFileType.Modified));
        result.AddRange(GetValues(changes.Conflicted, DiffFileType.Conflicted));
        result.AddRange(GetValues(changes.Copied, DiffFileType.Copied));
        result.AddRange(GetValues(changes.Renamed, DiffFileType.Renamed));
        result.AddRange(GetValues(changes.TypeChanged, DiffFileType.TypeChanged));
        result.AddRange(GetValues(changes.Unmodified, DiffFileType.Unmodified));

        return result;

        IEnumerable<DiffFileEntry> GetValues(IEnumerable<TreeEntryChanges> values, DiffFileType type)
        {
            return values.Select(s => new DiffFileEntry(s.Path, type));
        }
    }
}