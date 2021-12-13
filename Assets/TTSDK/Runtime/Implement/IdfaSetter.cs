using ByteDance.Union;
namespace TTSDK
{
    class IdfaSetter : IIdfaSetter
    {
        public void SetIdfa(string v)
        {
#if UNITY_IOS
            PangleConfiguration configuration = PangleConfiguration.CreateInstance();
            configuration.customIdfa = v;
#endif
        }
    }
}
