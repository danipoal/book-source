using BookSource.Models;
using Npgsql;

namespace BookSource.DAL
{
    public class PublicationDAL
    {

        private readonly IConfiguration _configuration;
        private string connectionString;
        public PublicationDAL(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public Publication GetPublicationById(int id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT * FROM Publication WHERE IdPublication = @IdPublication; ";
                NpgsqlCommand cmd = new NpgsqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@IdPublication", id);


                connection.Open();
                using(NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Publication
                        {
                            IdPublication =(int) reader["IdPublication"],
                            Title = (string)reader["Title"],
                            Content = (string)reader["Content"],
                            PubImage = reader.IsDBNull(reader.GetOrdinal("PubImage")) ? null : (string?)reader["PubImage"],
                            RIdUser = (int)reader["FkIdUser"]
                        };
                    }
                    return null;
                }

            }

        }
        public List<Publication> GetAllPublications()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT * FROM Publication";
                NpgsqlCommand cmd = new NpgsqlCommand(query, connection);
                List<Publication> publications = new List<Publication>();

                connection.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        publications.Add(new Publication()
                        {
                            IdPublication = (int)reader["IdPublication"],
                            Title = (string)reader["Title"],
                            Content = (string)reader["Content"],
                            PubImage = reader.IsDBNull(reader.GetOrdinal("PubImage")) ? null : (string?)reader["PubImage"],
                            RIdUser = (int)reader["FkIdUser"]
                        });
                    }
                    return publications;
                }
            }
        }
        public List<PublicationLikes> GetAllLikes()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT * FROM PublicationLikes";
                NpgsqlCommand cmd = new NpgsqlCommand(query, connection);
                List<PublicationLikes> pub_likes = new List<PublicationLikes>();

                connection.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pub_likes.Add(new PublicationLikes()
                        {
                            RIdPublication = (int)reader["FkIdPublication"],
                            RIdUser = (int)reader["FkIdUser"]
                        });
                    }
                    return pub_likes;
                }
            }
        }
        public Publication CreatePublication(Publication publication)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query;
                if (publication.PubImage != null)
                {
                    query = "INSERT INTO Publication(Title, Content, PubImage, FkIdUser) " +
                        "VALUES(@Title, @Content, @PubImage, @FkIdUser) " +
                        "RETURNING IdPublication;";
                }
                else
                {
                    query = "INSERT INTO Publication(Title, Content, FkIdUser) " +
                        "VALUES(@Title, @Content, @FkIdUser) " +
                        "RETURNING IdPublication;";
                }
                NpgsqlCommand cmd = new NpgsqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@Title", publication.Title);
                cmd.Parameters.AddWithValue("@Content", publication.Content);
                if (publication.PubImage != null)
                    cmd.Parameters.AddWithValue("@PubImage", publication.PubImage);
                cmd.Parameters.AddWithValue("@FkIdUser", publication.RIdUser);

                connection.Open();
                int idPublication;
                idPublication = Convert.ToInt32(cmd.ExecuteScalar());
                return GetPublicationById(idPublication);
            }
        }
        public int GetLikesByPublicationId(int publicationId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*)::int AS likes FROM PublicationLikes WHERE FkIdPublication= @FkIdPublication; ";
                NpgsqlCommand cmd = new NpgsqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@FkIdPublication", publicationId);
                connection.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (int)reader["likes"];
                    }
                    return 0;
                }
            }
        }
        public bool AddLikeToPublication(PublicationLikes publicationLike)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = "INSERT INTO PublicationLikes(FkIdPublication, FkIdUser) VALUES(@FkIdPublication, @FkIdUser) ";
                NpgsqlCommand cmd = new NpgsqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@FkIdPublication", publicationLike.RIdPublication);
                cmd.Parameters.AddWithValue("@FkIdUser", publicationLike.RIdUser);

                connection.Open();
                int affectedRows = 0;
                affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public bool DeleteLikeToPublication(PublicationLikes publicationLike)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = "DELETE FROM PublicationLikes WHERE FkIdPublication = @FkIdPublication AND FkIdUser = @FkIdUser ";
                NpgsqlCommand cmd = new NpgsqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@FkIdPublication", publicationLike.RIdPublication);
                cmd.Parameters.AddWithValue("@FkIdUser", publicationLike.RIdUser);

                connection.Open();
                int affectedRows = 0;
                affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    return true;
                }
                return false;
            }
        }

    }
}
