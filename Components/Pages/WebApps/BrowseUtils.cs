using MySql.Data.MySqlClient;
using System.Globalization;

namespace ShrineFoxCom.Components.Pages.WebApps
{
    public partial class Browse
    {
        private async Task<List<Post>> GetPostsFromTSV(string tsvPath)
        {
            var posts = new List<Post>();

            int i = 0;
            foreach (var line in File.ReadAllLines(tsvPath).Reverse())
            {
                var splitLines = line.Split('\t');
                string[] dateTimeFormats = { "M/d/yy", "M/d/yyyy" };

                var post = new Post();

                post.PostIndex = i++;
                post.Id = splitLines[1];
                post.Type = splitLines[2];
                post.Title = splitLines[3];
                post.Games = splitLines[4].Split(",");
                post.Authors = splitLines[5].Split(",");
                post.Date = DateTime.ParseExact(splitLines[6], dateTimeFormats, CultureInfo.InvariantCulture);
                post.Tags = splitLines[7].Split(",");
                post.Description = splitLines[8];
                post.UpdateText = splitLines[9];
                post.EmbedURL = splitLines[10];
                post.URL = splitLines[11];
                post.SourceURL = splitLines[12];

                posts.Add(post);
            }

            return posts.OrderBy(x => x.Date).ToList();
        }

        private async Task<Task> RemoveDuplicatePostData()
        {
            /*
                DELETE `a`
                FROM
                    `sf_browse` AS `a`,
                    `sf_browse` AS `b`
                WHERE
                    -- IMPORTANT: Ensures one version remains
                    -- Change "ID" to your unique column's name
                    `a`.`PostIndex` < `b`.`PostIndex`

                    -- Any duplicates you want to check for
                    AND (`a`.`ID` = `b`.`ID` OR `a`.`ID` IS NULL AND `b`.`ID` IS NULL)
            */

            return Task.CompletedTask;
        }


        private async Task<Task> UploadPostData()
        {
            if (alreadyRan)
                return Task.CompletedTask;

            string connectionString = Configuration.GetConnectionString("MySqlConnection");
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string insertQuery = "INSERT INTO sf_browse (Id, Type, Title, Games, Authors, Date, Tags, Description, EmbedURL, URL, UpdateText, SourceURL) VALUES (@Id, @Type, @Title, @Games, @Authors, @Date, @Tags, @Description, @EmbedURL, @URL, @UpdateText, @SourceURL)";

                foreach (var post in posts)
                {
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", post.Id);
                        command.Parameters.AddWithValue("@Type", post.Type);
                        command.Parameters.AddWithValue("@Title", post.Title);
                        command.Parameters.AddWithValue("@Games", string.Join(',', post.Games));
                        command.Parameters.AddWithValue("@Authors", string.Join(',', post.Authors));
                        command.Parameters.AddWithValue("@Date", post.Date);
                        command.Parameters.AddWithValue("@Description", post.Description);
                        command.Parameters.AddWithValue("@UpdateText", post.UpdateText);
                        command.Parameters.AddWithValue("@EmbedURL", post.EmbedURL);
                        command.Parameters.AddWithValue("@URL", post.URL);
                        command.Parameters.AddWithValue("@SourceURL", post.SourceURL);
                        command.Parameters.AddWithValue("@Tags", string.Join(',', post.Tags));

                        command.ExecuteNonQuery();
                        alreadyRan = true;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
