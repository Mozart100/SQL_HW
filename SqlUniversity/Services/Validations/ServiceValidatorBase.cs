﻿using ValidationResult = FluentValidation.Results.ValidationResult;

namespace SqlUniversity.Services.Validations
{
    public class UninversityException : Exception
    {
        public UninversityException(params UniversityError[] error)
        {
            Errors = error;
        }

        public UniversityError[] Errors { get; }
    }


    public abstract class ServiceValidatorBase
    {
        protected virtual IList<UniversityError> Dissect(ValidationResult validationResult)
        {
            var errors = new List<UniversityError>();
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    errors.Add(new UniversityError(errorMessage: error.ErrorMessage, propertyName: error.PropertyName));
                }
            }

            return errors;
        }
    }
}
