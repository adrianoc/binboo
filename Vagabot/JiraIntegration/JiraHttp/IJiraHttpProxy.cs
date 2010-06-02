namespace Binboo.JiraIntegration.JiraHttp
{
	interface IJiraHttpProxy
	{
		void Login(string userName, string password);
		bool IsLoggedIn { get; }
		void LinkIssues(int issueId, string linkDesc, string issueKey);
	}
}
