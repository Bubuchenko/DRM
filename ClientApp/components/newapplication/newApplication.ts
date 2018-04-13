import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';

interface Application {
    ID: string;
    Name: string;
    Description: string;
    CreationDate: number;
    Tasks: any;
}


@Component({
    components: {
        MenuComponent: require('../navmenu/navmenu.vue.html')
    }
})

export default class NewApplicationComponent extends Vue {
    Name: string = "";
    Description: string = "";

    @Prop({default: false})
    value!: boolean;

    hideCreateApplicationDialog() {
        this.$parent.$data.ShowCreateApplicationDialog = false;
    }

}
