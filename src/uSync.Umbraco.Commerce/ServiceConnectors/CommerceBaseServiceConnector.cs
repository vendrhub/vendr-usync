using System;
using System.Collections.Generic;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Deploy;


namespace uSync.Umbraco.Commerce.ServiceConnectors
{
    /// <summary>
    ///  this class does nothing, but the only way to add custom Udi values to umbraco
    ///  it to attach the attribute to a class implimenting IServiceConnector. 
    ///  so for brevity we do that attached to something that impliments this base class.
    /// </summary>
    public abstract class CommerceBaseServiceConnector : IServiceConnector
    {
        public bool Compare(IArtifact art1, IArtifact art2, ICollection<Difference> differences = null)
        {
            throw new NotImplementedException();
        }

        public void Explode(UdiRange range, List<Udi> udis)
        {
            throw new NotImplementedException();
        }

        public IArtifact GetArtifact(Udi udi)
        {
            throw new NotImplementedException();
        }

        public IArtifact GetArtifact(object entity)
        {
            throw new NotImplementedException();
        }

        public NamedUdiRange GetRange(Udi udi, string selector)
        {
            throw new NotImplementedException();
        }

        public NamedUdiRange GetRange(string entityType, string sid, string selector)
        {
            throw new NotImplementedException();
        }

        public void Process(ArtifactDeployState dart, IDeployContext context, int pass)
        {
            throw new NotImplementedException();
        }

        public ArtifactDeployState ProcessInit(IArtifact art, IDeployContext context)
        {
            throw new NotImplementedException();
        }
    }
}
