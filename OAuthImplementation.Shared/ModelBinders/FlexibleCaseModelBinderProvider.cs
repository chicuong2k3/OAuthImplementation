using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace OAuthImplementation.Shared.ModelBinders
{
    public class FlexibleCaseModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return new BinderTypeModelBinder(typeof(FlexibleCaseModelBinder));
        }
    }
}
