namespace Adminpanel.api.Domain.Enum
{
    public class ContentEnum
    {
        public enum PostStatus { Draft, Published }
        public enum PageStatus { Draft, Published }
        public enum PlacementPosition { Bottom, Sidebar, Header }
        public enum PlacementContentType { Page, Blog, External }
        public enum UserRole { Editor, Admin }

    }
}
