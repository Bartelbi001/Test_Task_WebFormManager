using System.Text.Json;

namespace WebFormManager.API.Interfaces;

public interface ISubmissionValidator
{
    void Validate(JsonElement data);
}