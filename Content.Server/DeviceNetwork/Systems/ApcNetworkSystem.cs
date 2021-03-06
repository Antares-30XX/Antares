using Content.Server.DeviceNetwork.Components;
using Content.Server.NodeContainer;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;
using Content.Server.Power.EntitySystems;
using Content.Server.Power.Nodes;
using Robust.Shared.IoC;

namespace Content.Server.DeviceNetwork.Systems
{
    [UsedImplicitly]
    public class ApcNetworkSystem : EntitySystem
    {
        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<ApcNetworkComponent, BeforePacketSentEvent>(OnBeforePacketSent);

            SubscribeLocalEvent<ApcNetworkComponent, ExtensionCableSystem.ProviderConnectedEvent>(OnProviderConnected);
            SubscribeLocalEvent<ApcNetworkComponent, ExtensionCableSystem.ProviderDisconnectedEvent>(OnProviderDisconnected);
        }

        /// <summary>
        /// Checks if both devices are connected to the same apc
        /// </summary>
        private void OnBeforePacketSent(EntityUid uid, ApcNetworkComponent receiver, BeforePacketSentEvent args)
        {
            if (!EntityManager.TryGetComponent(args.Sender, out ApcNetworkComponent? sender)) return;

            if (sender.ConnectedNode?.NodeGroup == null || !sender.ConnectedNode.NodeGroup.Equals(receiver.ConnectedNode?.NodeGroup))
            {
                args.Cancel();
            }
        }

        private void OnProviderConnected(EntityUid uid, ApcNetworkComponent component, ExtensionCableSystem.ProviderConnectedEvent args)
        {
            if (!EntityManager.TryGetComponent(args.Provider.Owner, out NodeContainerComponent? nodeContainer)) return;

            if (nodeContainer.TryGetNode("power", out CableNode? node))
            {
                component.ConnectedNode = node;
            }
            else if (nodeContainer.TryGetNode("output", out CableDeviceNode? deviceNode))
            {
                component.ConnectedNode = deviceNode;
            }

        }

        private void OnProviderDisconnected(EntityUid uid, ApcNetworkComponent component, ExtensionCableSystem.ProviderDisconnectedEvent args)
        {
            component.ConnectedNode = null;
        }
    }
}
