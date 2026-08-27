namespace WeatherDashboard.Application.Validators;

using FluentValidation;
using WeatherDashboard.Application.DTOs;

public class WeatherHistoryFilterValidator : AbstractValidator<WeatherHistoryFilterDto>
{
    public WeatherHistoryFilterValidator()
    {
        RuleFor(x => x.CityId)
            .GreaterThan(0)
            .WithMessage("O ID da cidade deve ser maior que zero.");

        RuleFor(x => x)
            .Must(x => !x.StartDateUtc.HasValue || !x.EndDateUtc.HasValue || x.StartDateUtc.Value <= x.EndDateUtc.Value)
            .WithMessage("A data inicial não pode ser posterior à data final.");

        RuleFor(x => x)
            .Must(x =>
            {
                if (x.StartDateUtc.HasValue && x.EndDateUtc.HasValue)
                {
                    return (x.EndDateUtc.Value - x.StartDateUtc.Value).TotalDays <= 366;
                }
                return true;
            })
            .WithMessage("O intervalo de datas pesquisado não pode exceder 366 dias.");
    }
}
