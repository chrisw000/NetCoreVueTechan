import Chart from 'components/chart'
import TradeChart from 'components/charts/tradeChart'

export const routes = [
    { path: '/', redirect: '/chart/0' },
    {
         name: 'chart', 
         path: '/chart', 
         component: Chart, 
         children: [{
             path: ':overlayId',
             component: TradeChart,
             props: true
         }]
    }
];
