import HomePage from 'components/home-page'
import Chart from 'components/chart'

export const routes = [
  { name: 'home', path: '/', component: HomePage, display: 'Home', icon: 'home' },
    {
         name: 'chart', 
         path: '/chart/:overlayId', 
         component: Chart, 
         display: 'Chart', 
         icon: 'chart-line',
         props: true
    }
];
