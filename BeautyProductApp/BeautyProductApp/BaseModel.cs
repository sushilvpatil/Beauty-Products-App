using System;
using System.Collections.Generic;
using System.Text;
using FreshMvvm;
namespace BeautyProductApp
{
    public class BaseModel:FreshBasePageModel
    {
        public string GetStringByInteger(int value)
        {
            try
            {
                return value.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
