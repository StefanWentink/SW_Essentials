﻿namespace SWE.Contract.Test.Data
{
    using System;

    internal static class PersonFactory
    {
        internal static PersonStub Create()
        {
            return Create("Arnold", null);
        }

        internal static PersonStub Create(string firstName, string initials)
        {
            return new PersonStub(firstName, "Schwarzenegger", new DateTime(1947, 7, 30))
            {
                Initials = initials
            };
        }
    }
}