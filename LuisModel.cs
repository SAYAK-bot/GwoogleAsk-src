using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot
{

    public class Intent
    {
        public string name { get; set; }
    }

    public partial class Entity
    {
        public string name { get; set; }
        public IList<object> roles { get; set; }
    }

    public partial class Entity
    {
        public string entity { get; set; }
        public int startPos { get; set; }
        public int endPos { get; set; }
    }

    public class Utterance
    {
        public string text { get; set; }
        public string intent { get; set; }
        public IList<Entity> entities { get; set; }
    }

    public class LuisModel
    {
        public string luis_schema_version { get; set; }
        public string versionId { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string culture { get; set; }
        public string tokenizerVersion { get; set; }
        public IList<Intent> intents { get; set; }
        public IList<Entity> entities { get; set; }
        public IList<object> composites { get; set; }
        public IList<object> closedLists { get; set; }
        public IList<object> patternAnyEntities { get; set; }
        public IList<object> regex_entities { get; set; }
        public IList<object> prebuiltEntities { get; set; }
        public IList<object> model_features { get; set; }
        public IList<object> regex_features { get; set; }
        public IList<object> patterns { get; set; }
        public IList<Utterance> utterances { get; set; }
        public IList<object> settings { get; set; }
    }
}
