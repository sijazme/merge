using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace merge
{
    class Template
    {
        public string BlankFemale { get { return GetBlankFemale(); } }

        public string BlankMale { get { return GetBlankMale(); } }

        public string GetBlankFemale()
        {
            return
@"J
1
1
1
1
1
y
y
y
y
5
2
2
2
3
G                                                                                                                                                                                                           2
G
L
T
T
I
H
Y
Y
[
B";

        }


        public string GetBlankMale()
        {
            return
@"";

        }
    }
}
