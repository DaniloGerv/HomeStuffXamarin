using System;
using System.Collections.Generic;
using Plugin.CloudFirestore.Attributes;

namespace homestuff.entities
{
    public class Family
    {
        [Id]
        public string id { get; set; }
        [MapTo("fid")]
        public string fid { get; set; }
        [MapTo("familyName")]
        public string familyName { get; set; }
        [MapTo("membersID")]
        public List<string> membersID { get; set; }
        [MapTo("place")]
        public Location place { get; set; }
        [MapTo("toDoList")]
        public List<Domesticas> toDoList { get; set; }


        public Family() {
            this.membersID = new List<string>();
            this.toDoList = new List<Domesticas>();

        }

        public void addMember(string u)
        {
            if (membersID == null)
                membersID = new List<string>();
            membersID.Add(u);
        }

        public void removeMember(string u)
        {
            if (membersID != null)
            {
                for (int i = 0; i < membersID.Count; i++)
                    if (membersID[i].Equals(u))
                        membersID.RemoveAt(i);
            }
        }

       

        public void addDomesticas(Domesticas d)
        {
            if (this.toDoList == null)
                this.toDoList = new List<Domesticas>();
            this.toDoList.Add(d);
        }

        public void removeDomestica(Domesticas d)
        {
            if (toDoList!=null)
            foreach(Domesticas dom in toDoList)
            {
                if (dom.domesticaID.Equals(d.domesticaID))
                {
                    toDoList.Remove(dom);
                }
            }
        }

        public void reset()
        {
            if (toDoList != null)
                this.toDoList.Clear();
            this.familyName = "";
            if (membersID != null)
                this.membersID.Clear();
        }
    }
}
