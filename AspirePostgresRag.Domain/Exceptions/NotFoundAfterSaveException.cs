namespace AspirePostgresRag.Models.Exceptions;

public class NotFoundAfterSaveException(int id): Exception($"Entity with id {id} was not found after save.")
{
    
}
