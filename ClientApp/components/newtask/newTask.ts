import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';



@Component({
    components: {
        TaskFirstStepComponent: require('../firststep/firststep.vue.html'),
        TaskSecondStepComponent: require('../secondstep/secondstep.vue.html')
    }
})
export default class NewTaskComponent extends Vue {
    $route: any;
    $http: any;
    stepProgress: any = 2;


    @Prop({ default: false })
    value!: boolean;

    close() {
        this.$emit('close');
    }
}
