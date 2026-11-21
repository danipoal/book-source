using BookSource.Models;
using System.Data;
using Npgsql;

namespace BookSource.DAL
{
    public class ListOfBooksDAL
    {
        private readonly IConfiguration _configuration;
        private string? connectionString;
        public ListOfBooksDAL(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public List<ListOfBooks> GetListsOfByUserId(int UserId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT * FROM ListOfBooks WHERE FkIdUser = @FkIdUser";
                NpgsqlCommand cmd = new NpgsqlCommand (query, conn);

                cmd.Parameters.AddWithValue("@FkIdUser", UserId);
                conn.Open();

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    List<ListOfBooks> list = new List<ListOfBooks>();
                    while (reader.Read())
                    {
                        ListOfBooks listOfBooks = new ListOfBooks
                        {
                            IdListOfBooks = (int)reader["IdListOfBooks"],
                            ListName = (string)reader["ListName"],
                            RIdUser = (int)reader["FkIdUser"]
                        };
                        list.Add(listOfBooks);
                    }
                    return list;
                }
            }
        }

        public ListOfBooks GetListOfBooksById(int ListOfBookId)
        {
            using(NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT * FROM ListOfBooks WHERE IdListOfBooks = @IdListOfBooks;";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@IdListOfBooks", ListOfBookId);
                conn.Open();

                using(NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ListOfBooks
                        {
                            IdListOfBooks = (int)reader["IdListOfBooks"],
                            ListName = (string)reader["ListName"],
                            RIdUser = (int)reader["FkIdUser"]
                        };
                    }
                    return null;
                }
            }
        }

        public bool CreateListOfBooks(ListOfBooks listOfBooks)
        {
            using(NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                string query = "INSERT INTO ListOfBooks(ListName, FkIdUser)" +
                    "VALUES(@ListName, @FkIdUser)";
                NpgsqlCommand cmd = new NpgsqlCommand (query, conn);

                cmd.Parameters.AddWithValue("@ListName", listOfBooks.ListName); 
                cmd.Parameters.AddWithValue("@FkIdUser", listOfBooks.RIdUser);
                conn.Open();

                int affectedRows = 0;

                affectedRows = cmd.ExecuteNonQuery();

                if (affectedRows > 0)
                    return true;

                return false;
            }
        }

        // Tiene OnDeleteCascade de manera que si eliminamos una lista, tambien se eliminan los registros de
        // libros de esa lista
        public bool RemoveListOfBooks(ListOfBooks listOfBooks)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                string query = "DELETE FROM ListOfBooks WHERE IdListOfBooks = @IdListOfBooks";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@IdListOfBooks", listOfBooks.IdListOfBooks);
                conn.Open();

                int affectedRows = 0;

                affectedRows = cmd.ExecuteNonQuery();

                if (affectedRows > 0)
                    return true;

                return false;
            }
        }

        // No modificar el Id, sino no funcionará
        public ListOfBooks UpdateListOfBooks(ListOfBooks listOfBooks)
        {
            using(NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                string query = "UPDATE ListOfBooks SET ListName = @ListName " +
                    "WHERE IdListOfBooks = @IdListOfBooks;";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@IdListOfBooks", listOfBooks.IdListOfBooks);
                cmd.Parameters.AddWithValue("@ListName", listOfBooks.ListName);
                conn.Open();

                int affectedRows = 0;
                affectedRows =  cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    return listOfBooks;
                }
                return null;
            }

        }
        public bool AddBookToListOfBooks(int idListOfBooks,int idBook)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                string query = $"INSERT INTO ListOfBooks_Book VALUES(@IdListOfBooks,@IdBook)";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@IdListOfBooks", idListOfBooks);
                cmd.Parameters.AddWithValue("@IdBook", idBook);
                conn.Open();

                int affectedRows = 0;
                affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public bool DeleteBookToListOfBooks(int idListOfBooks, int idBook)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                string query = $"DELETE FROM ListOfBooks_Book " +
                    $"WHERE FkIdBook = @IdBook AND FkIdListOfBooks = @IdListOfBooks";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@IdBook", idBook);
                cmd.Parameters.AddWithValue("@IdListOfBooks", idListOfBooks);
                conn.Open();
                string queryDebug = DebugSqlCommand(cmd);
                int affectedRows = 0;
                affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    return true;
                }
                return false;
            }
        }
        string DebugSqlCommand(NpgsqlCommand command)
        {
            string debugQuery = command.CommandText;
            foreach (NpgsqlParameter param in command.Parameters)
            {
                string paramValue = param.Value == null ? "NULL" : param.Value.ToString();
                debugQuery = debugQuery.Replace(param.ParameterName, $"'{paramValue}'");
            }
            return debugQuery;
        }

        public ListOfBooks AddBookToListBook(ListOfBooks listOfBooks, Book book)
        {
            using(NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                string query = "INSERT INTO ListOfBooks_Book(FkIdListOfBooks, FkIdBook) " +
                    "VALUES(@FkIdListOfBooks, @FkIdBook)";
                NpgsqlCommand cmd = new NpgsqlCommand(query,conn);

                cmd.Parameters.AddWithValue("@FkIdListOfBooks", listOfBooks.IdListOfBooks);
                cmd.Parameters.AddWithValue("@FkIdBook", book.IdBook);
                conn.Open();

                int affectedRows = 0;
                affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    return listOfBooks;
                }
                return null;

            }
        }

    }
}
