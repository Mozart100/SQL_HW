using Microsoft.AspNetCore.Mvc;
using SqlUniversity.Model.Requests;
using SqlUniversity.Services.Validations;

namespace SqlUniversity.Controllers
{
    public class UniversityControllerBase : ControllerBase
    {
        protected async Task<TResponse> ErrorWrapper<TRequest, TResponse>(Func<Task<TResponse>> callback, Func<TRequest> exceptionCallback = null) where TResponse : UniversityReponseBase<TRequest>, new()
                                                                                                          where TRequest : class
        {
            TResponse response = null;
            try
            {
                response = await callback();
            }
            catch (UninversityException universityException)
            {
                response = new TResponse
                {
                    ErrorSection = new ErrorSection
                    {
                        Message = "There was a problem please resolve it",
                        Errors = universityException.Errors

                    },
                    Request = exceptionCallback?.Invoke(),
                    IsOperationPassed = false
                };
            }
            catch (Exception ex)
            {
                response = new TResponse();
                response.ErrorSection = new ErrorSection { Message = ex.Message };
                response.IsOperationPassed = false;
            }


            return response;
        }

        protected async Task<TResponse> ErrorWrapper<TRequest, TResponse>( TRequest request, Func<TRequest,Task<TResponse>> callback) where TResponse : UniversityReponseBase<TRequest>, new()
                                                                                                          where TRequest : class
        {
            TResponse response = null;
            try
            {
                response = await callback(request);
            }
            catch (UninversityException universityException)
            {
                response = new TResponse
                {
                    ErrorSection = new ErrorSection
                    {
                        Message = "There was a problem please resolve it",
                        Errors = universityException.Errors

                    },
                    Request = request,
                    IsOperationPassed = false
                };
            }
            catch (Exception ex)
            {
                response = new TResponse();
                response.ErrorSection = new ErrorSection { Message = ex.Message };
                response.IsOperationPassed = false;
            }


            return response;
        }

    }
}