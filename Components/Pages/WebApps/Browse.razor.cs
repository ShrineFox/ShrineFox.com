using Microsoft.AspNetCore.Components;
using MySql.Data.MySqlClient;
using PersonaGameLib;
using System.Data;
using System.Globalization;
using System.Web;

namespace ShrineFoxCom.Components.Pages.WebApps
{
    public partial class Browse
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private IConfiguration Configuration { get; set; }


        private List<Game> games = new List<Game>();
        private string selectedGameShortName = "";
        private string selectedPostType = "tool";
        private string selectedAuthor = "";
        private string selectedPostId = "";
        private int postsPerPage = 15;
        private int currentPage = 1; // Ensure currentPage is set to 1
        private List<Post> posts = new List<Post>();
        private List<Post> curatedPosts = new List<Post>();
        private List<Post> pagePosts = new List<Post>();
        private bool alreadyRan = false;

        public class Post
        {
            public int PostIndex { get; set; } = 0;
            public string Id { get; set; } = "";
            public string Type { get; set; } = "";
            public string Title { get; set; } = "";
            public string[] Games { get; set; } = Array.Empty<string>();
            public string[] Authors { get; set; } = Array.Empty<string>();
            public DateTime Date { get; set; } = new DateTime();
            public string[] Tags { get; set; } = Array.Empty<string>();
            public string Description { get; set; } = "";
            public string EmbedURL { get; set; } = "";
            public string URL { get; set; } = "";
            public string UpdateText { get; set; } = "";
            public string SourceURL { get; set; } = "";
        }

        protected override async Task OnInitializedAsync()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var query = HttpUtility.ParseQueryString(uri.Query);

            selectedGameShortName = query["game"] ?? "";
            selectedPostType = query["type"] ?? "";
            selectedAuthor = query["author"] ?? "";
            selectedPostId = query["post"] ?? "";
            currentPage = int.TryParse(query["page"], out var page) ? page : 1;

            foreach (var platform in PersonaGameLib.PersonaGames.Platforms)
            {
                foreach (var game in platform.Games.Where(x => x.Region == "USA" || x.Platform == "PC"))
                {
                    games.Add(game);
                }
            }

            /* posts = await GetPostsFromTSV(@"C:\Users\ShrineFox\Documents\amicitia.tsv");
            await UploadPostData();
            await RemoveDuplicatePostData(); */

            posts = await GetPostsFromDatabaseAsync();
            posts.Sort((x, y) => DateTime.Compare(x.Date, y.Date));
            posts.Reverse();
            UpdateCuratedPosts();
            UpdatePagePosts(); // Ensure the first page of results is loaded
        }

        private async Task<List<Post>> GetPostsFromDatabaseAsync()
        {
            var posts = new List<Post>();

            string connectionString = Configuration.GetConnectionString("MySqlConnection");
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT PostIndex, Id, Type, Title, Games, Authors, Date, Tags, Description, EmbedURL, URL, UpdateText, SourceURL FROM sf_browse";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var post = new Post
                            {
                                PostIndex = reader.GetInt32("PostIndex"),
                                Id = reader.GetString("Id"),
                                Type = reader.GetString("Type"),
                                Title = reader.GetString("Title"),
                                Games = reader.GetString("Games").Split(','),
                                Authors = reader.GetString("Authors").Split(','),
                                Date = reader.GetDateTime("Date"),
                                Tags = reader.GetString("Tags").Split(','),
                                Description = reader.GetString("Description"),
                                EmbedURL = reader.GetString("EmbedURL"),
                                URL = reader.GetString("URL"),
                                UpdateText = reader.GetString("UpdateText"),
                                SourceURL = reader.GetString("SourceURL")
                            };
                            posts.Add(post);
                        }
                    }
                }
            }

            return posts;
        }

        private async Task _SelectedGameChanged()
        {
            currentPage = 1;
            UpdateCuratedPosts();
            UpdatePagePosts();
        }

        private void UpdateCuratedPosts()
        {
            curatedPosts = posts;

            if (selectedPostType != "")
                curatedPosts = curatedPosts.Where(x => x.Type.ToLower() == selectedPostType.ToLower()).ToList();
            if (selectedGameShortName != "")
                curatedPosts = curatedPosts.Where(x => x.Games.Any(y => y.ToLower().Equals(selectedGameShortName.ToLower()))).ToList();
            if (selectedAuthor != "")
                curatedPosts = curatedPosts.Where(x => x.Authors.Any(y => y.ToLower().Equals(selectedAuthor.ToLower()))).ToList();
            if (selectedPostId != "")
                curatedPosts = curatedPosts.Where(x => x.Id.ToLower() == selectedPostId.ToLower()).ToList();
        }

        private void UpdatePagePosts()
        {
            pagePosts = curatedPosts.Skip((currentPage - 1) * postsPerPage)
                .Take(postsPerPage).ToList();
        }

        private async Task _SelectedTypeChanged()
        {
            currentPage = 1;
            UpdateCuratedPosts();
            UpdatePagePosts();
        }

        private void HandlePageChange(int selectedPage)
        {
            currentPage = selectedPage;
            UpdatePagePosts();
        }
    }
}
