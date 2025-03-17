import { createRouter, createWebHistory } from 'vue-router';
import AboutPage from '../pages/AboutPage.vue';
import FormPage from '../pages/FormPage.vue';
import ListPage from '../pages/ListPage.vue';

const routes = [
    { path: '/main', component: AboutPage },
    { path: '/form', component: FormPage },
    { path: '/list', component: ListPage },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;
