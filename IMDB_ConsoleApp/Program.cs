using System;
using System.Data;
using Microsoft.Data.SqlClient;

class Program
{
    static string connectionString = "Server=localhost;Database=IMDB_3;Trusted_Connection=True;TrustServerCertificate=True;";


    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\nVælg en handling:");
            Console.WriteLine("1: Opret film");
            Console.WriteLine("2: Opret person");
            Console.WriteLine("3: Find film");
            Console.WriteLine("4: Find person");
            Console.WriteLine("5: Find film for en skuespiller"); 
            Console.WriteLine("6: Slet film");
            Console.WriteLine("7: Slet person");
            Console.WriteLine("8: Afslut");
            Console.Write("Indtast dit valg: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    InsertMovie();
                    break;
                case "2":
                    InsertPerson();
                    break;
                case "3":
                    SearchMovies();
                    break;
                case "4":
                    SearchPersons();
                    break;
                case "5":
                    SearchMoviesByActor();
                    break;
                case "6":
                    DeleteMovie();
                    break;
                case "7":
                    DeletePerson();
                    break;
                case "8":
                    return;
                default:
                    Console.WriteLine("Ugyldigt valg, prøv igen.");
                    break;
            }
        }
    }

    static void InsertMovie()
    {
        Console.Write("Indtast tconst (unik film ID, f.eks. tt1234567): ");
        string tconst = Console.ReadLine();

        Console.Write("Indtast titleType (f.eks. movie, short, tvSeries): ");
        string titleType = Console.ReadLine();

        Console.Write("Indtast primaryTitle: ");
        string primaryTitle = Console.ReadLine();

        Console.Write("Indtast originalTitle: ");
        string originalTitle = Console.ReadLine();

        Console.Write("Er det en voksenfilm? (0 = Nej, 1 = Ja): ");
        bool isAdult = Console.ReadLine() == "1";

        Console.Write("Indtast startYear (eller tryk Enter for NULL): ");
        int? startYear = ReadNullableInt();

        Console.Write("Indtast endYear (eller tryk Enter for NULL): ");
        int? endYear = ReadNullableInt();

        Console.Write("Indtast runtimeMinutes (eller tryk Enter for NULL): ");
        int? runtimeMinutes = ReadNullableInt();

        Console.Write("Indtast genrer (kommasepareret, f.eks. Action,Drama): ");
        string genres = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("sp_InsertMovieSimple", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tconst", tconst);
            cmd.Parameters.AddWithValue("@titleType", titleType);
            cmd.Parameters.AddWithValue("@primaryTitle", primaryTitle);
            cmd.Parameters.AddWithValue("@originalTitle", originalTitle);
            cmd.Parameters.AddWithValue("@isAdult", isAdult);
            cmd.Parameters.AddWithValue("@startYear", (object)startYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@endYear", (object)endYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@runtimeMinutes", (object)runtimeMinutes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@genres", string.IsNullOrWhiteSpace(genres) ? DBNull.Value : genres);

            conn.Open();
            cmd.ExecuteNonQuery();
            Console.WriteLine("Filmen blev oprettet.");
        }
    }
    static void SearchMoviesByActor()
    {
        Console.Write("Indtast skuespillerens navn: ");
        string actorName = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("GetMoviesByActor", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ActorName", actorName);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\n🎬 Filmresultater for " + actorName + ":");
            bool found = false;
            while (reader.Read())
            {
                found = true;
                Console.WriteLine($"- {reader["primaryTitle"]} ({reader["startYear"]}) - ID: {reader["tconst"]}");
            }

            if (!found)
            {
                Console.WriteLine("Ingen film fundet for denne skuespiller.");
            }
        }
    }

    static void InsertPerson()
    {
        Console.Write("Indtast nconst (unik person ID, f.eks. nm1234567): ");
        string nconst = Console.ReadLine();

        Console.Write("Indtast primaryName (f.eks. Leonardo DiCaprio): ");
        string primaryName = Console.ReadLine();

        Console.Write("Indtast birthYear (eller tryk Enter for NULL): ");
        int? birthYear = ReadNullableInt();

        Console.Write("Indtast deathYear (eller tryk Enter for NULL): ");
        int? deathYear = ReadNullableInt();

        Console.Write("Indtast knownForTitles (kommasepareret tconst, fx tt1234567,tt7654321): ");
        string knownFor = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("sp_InsertPerson", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nconst", nconst);
            cmd.Parameters.AddWithValue("@primaryName", primaryName);
            cmd.Parameters.AddWithValue("@birthYear", (object)birthYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@deathYear", (object)deathYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@knownForTitles", string.IsNullOrWhiteSpace(knownFor) ? DBNull.Value : knownFor);

            conn.Open();
            cmd.ExecuteNonQuery();
            Console.WriteLine("Personen blev oprettet.");
        }
    }

    static void SearchMovies()
    {
        Console.Write("Indtast søgeord for film, fx titel årstal, eller titel, eller årstal: ");
        string searchTerm = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("sp_SearchMovies", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@searchTerm", searchTerm);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("\n🎬 Filmresultater:");
            while (reader.Read())
            {
                Console.WriteLine($"- {reader["primaryTitle"]} ({reader["startYear"]}) - ID: {reader["tconst"]}");
            }
        }
    }

    static void SearchPersons()
    {
        Console.Write("Indtast søgeord for person: ");
        string searchTerm = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("sp_SearchPersons", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@searchTerm", searchTerm);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("\n👤 Personresultater:");
            while (reader.Read())
            {
                string deathYear = reader["deathYear"] != DBNull.Value ? reader["deathYear"].ToString() : "N/A";
                Console.WriteLine($"- {reader["primaryName"]} (Født: {reader["birthYear"]}, Død: {deathYear}) - ID: {reader["nconst"]}");
            }
        }
    }

    static void DeleteMovie()
    {
        Console.Write("Indtast tconst for filmen du vil slette: ");
        string tconst = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("sp_DeleteMovie", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tconst", tconst);

            conn.Open();
            cmd.ExecuteNonQuery();
            Console.WriteLine("Filmen blev slettet.");
        }
    }

    static void DeletePerson()
    {
        Console.Write("Indtast nconst for personen du vil slette: ");
        string nconst = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("sp_DeletePerson", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nconst", nconst);

            conn.Open();
            cmd.ExecuteNonQuery();
            Console.WriteLine("Personen blev slettet.");
        }
    }

    static int? ReadNullableInt()
    {
        string input = Console.ReadLine();
        return int.TryParse(input, out int value) ? value : (int?)null;
    }
}
