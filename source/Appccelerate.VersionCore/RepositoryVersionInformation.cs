namespace Appccelerate.Version
{
    public struct RepositoryVersionInformation
    {
        public RepositoryVersionInformation(
            string lastTaggedVersion, 
            int commitsSinceLastTaggedVersion, 
            string annotationMessage, 
            string prereleaseOverride)
            : this()
        {
            this.LastTaggedVersion = lastTaggedVersion;
            this.CommitsSinceLastTaggedVersion = commitsSinceLastTaggedVersion;
            this.AnnotationMessage = annotationMessage;
            this.PrereleaseOverride = prereleaseOverride;
        }

        public string LastTaggedVersion { get; private set; }

        public string AnnotationMessage { get; private set; }

        public int CommitsSinceLastTaggedVersion { get; private set; }

        public string PrereleaseOverride { get; private set; }
    }
}