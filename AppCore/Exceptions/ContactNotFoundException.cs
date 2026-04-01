using System;
using System.Collections.Generic;
namespace AppCore.Exceptions;


public class ContactNotFoundException: Exception
{
    public ContactNotFoundException(string msg): base(msg)
    {
    }
}


    

    
