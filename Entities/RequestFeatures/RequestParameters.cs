﻿namespace Entities.RequestFeatures
{
    public abstract class RequestParameters
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; }
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
        public String? OrderBy { get; set; }
        public String? Fields { get; set; }
    }
}
