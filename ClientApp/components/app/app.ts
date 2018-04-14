import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import MenuComponent from './../navmenu/navmenu';


@Component({
    components: {
        MenuComponent: require('../navmenu/navmenu.vue.html'),
    }
})
export default class AppComponent extends Vue {
    drawer: boolean = false;
    darkTheme: boolean = true;
}
