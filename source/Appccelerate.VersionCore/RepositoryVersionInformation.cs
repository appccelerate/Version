namespace Appccelerate.Version
{
    public struct RepositoryVersionInformation
    {
        public RepositoryVersionInformation(string lastTaggedVersion, int commitsSinceLastTaggedVersion, string annotationMessage, bool isPullRequest)
            : this()
        {
            this.LastTaggedVersion = lastTaggedVersion;
            this.CommitsSinceLastTaggedVersion = commitsSinceLastTaggedVersion;
            this.AnnotationMessage = annotationMessage;
            this.IsPullRequest = isPullRequest;
        }

        public string LastTaggedVersion { get; private set; }

        public string AnnotationMessage { get; private set; }

        public int CommitsSinceLastTaggedVersion { get; private set; }

        public bool IsPullRequest { get; private set; }
    }
}