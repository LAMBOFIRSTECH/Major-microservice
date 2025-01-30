using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksManagement_API.Middlewares
{
	public class ContextPathMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly PathString _contextPath;

		public ContextPathMiddleware(RequestDelegate next, string contextPath)
		{
			_next = next;
			_contextPath = new PathString(contextPath);
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (context.Request.Path.StartsWithSegments(_contextPath, out var remainingPath))
		   {     
				context.Request.Path = remainingPath;
				await _next(context); 
			}
			else
			{
				context.Response.StatusCode = StatusCodes.Status404NotFound;
			}
		}
	}


}
