using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Domain.Servicos {
    public class ServicoDuplicacaoNif : IServicoDuplicacaoNif {
        public static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Converters = new JsonConverter[] { new StringEnumConverter() }
        };

        private const string _nomeProjecao = "NifsDirecoes";
        private readonly ProjectionsManager _gestor;

        public ServicoDuplicacaoNif(ProjectionsManager gestor) {
            Contract.Requires(gestor != null);
            Contract.Ensures(_gestor != null);
            _gestor = gestor;
        }

        public async Task<bool> NifDuplicado(string nif, Guid id) {
            string estado = await _gestor.GetStateAsync(_nomeProjecao);
            if (string.IsNullOrEmpty(estado)) {
                estado = "{\"ids\":[] }";
            }
            var lista = JsonConvert.DeserializeObject<Lista>(estado, _jsonSettings);
            return lista.ids != null && lista.ids.Length > 0 && lista.ids.Any(i => i.id == id && i.nif == nif);
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_gestor != null);
        }

        private class Info {
            public Guid id;
            public string nif;
        }

        private class Lista {
            public Info[] ids;
        }
    }
}