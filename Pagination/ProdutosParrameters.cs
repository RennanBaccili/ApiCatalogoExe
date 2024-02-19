﻿namespace ApiCatalogo.Pagination
{
    public class ProdutosParrameters
    {
        const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _PageSize;

        public int PageSize
        {
            get
            {
                return _PageSize;
            }
            set
            {
                _PageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }
    }
}