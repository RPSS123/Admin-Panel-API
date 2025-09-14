﻿namespace Adminpanel.api.DTOs
{
        public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
}
