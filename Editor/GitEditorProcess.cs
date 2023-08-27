using System;
using System.Diagnostics;

namespace GameLovers.Services.Editor
{
	/// <summary>
	/// Run git commands processes that would otherwise be used in the terminal.
	/// </summary>
	/// <author>
	/// https://blog.somewhatabstract.com/2015/06/22/getting-information-about-your-git-repository-with-c/
	/// </author>
	public class GitEditorProcess : IDisposable
	{
		private const string DefaultPathToGitBinary = "git";

		private readonly Process Process;

		/// <summary>
		/// <inheritdoc cref="Process.ExitCode"/>
		/// </summary>
		public int ExitCode => Process.ExitCode;

		public GitEditorProcess(string workingDir, string pathToGitBinary = DefaultPathToGitBinary)
		{
			var startInfo = new ProcessStartInfo
			{
				UseShellExecute = false,
				RedirectStandardOutput = true,
				FileName = pathToGitBinary,
				CreateNoWindow = true,
				WorkingDirectory = workingDir
			};

			Process = new Process { StartInfo = startInfo };
		}

		/// <summary>
		/// Is this unity project a git repo?
		/// </summary>
		public bool IsValidRepo()
		{
			return ExecuteCommand("rev-parse --is-inside-work-tree") == "true";
		}

		/// <summary>
		/// Get the git branch name of the unity project.
		/// </summary>
		public string GetBranch()
		{
			return ExecuteCommand("rev-parse --abbrev-ref HEAD");
		}

		/// <summary>
		/// Get the git commit hash of the unity project.
		/// </summary>
		public string GetCommitHash()
		{
			return ExecuteCommand($"rev-parse HEAD");
		}

		/// <summary>
		/// Get the diff of the working directory in its current state from the state it was at at
		/// a given commit.
		/// </summary>
		public string GetDiffFromCommit(string commitHash)
		{
			return ExecuteCommand($"diff --word-diff=porcelain {commitHash} -- {Process.StartInfo.WorkingDirectory}");
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Process?.Dispose();
		}
		
		/// <summary>
		/// Execute a command eg. "status --verbose"
		/// </summary>
		private string ExecuteCommand(string args)
		{
			Process.StartInfo.Arguments = args;
			Process.Start();
			var output = Process.StandardOutput.ReadToEnd().Trim();
			Process.WaitForExit();
			return output;
		}
	}
}
