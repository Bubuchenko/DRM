import Vue from 'vue';
import { Component } from 'vue-property-decorator';

@Component
export default class MenuComponent extends Vue {
    drawer: boolean = false;

    switchTheme() {
        this.$emit('switch');
    }
}