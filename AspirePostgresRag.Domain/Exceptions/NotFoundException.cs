namespace AspirePostgresRag.Models.Exceptions;

public class NotFoundException(int id): Exception($"Entity with id {id} was not found.")
{
    
}
