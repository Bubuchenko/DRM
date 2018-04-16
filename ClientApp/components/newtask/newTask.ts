import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';



@Component({
    components: {
        TaskFirstStepComponent: require('../firststep/firststep.vue.html'),
        TaskSecondStepComponent: require('../secondstep/secondstep.vue.html'),
        TaskThirdStepComponent: require('../thirdstep/thirdstep.vue.html')
    }
})
export default class NewTaskComponent extends Vue {
    $route: any;
    $http: any;
    stepProgress: any = 3;
    selectedConfiguration: number = 0;

    @Prop({ default: false })
    value!: boolean;

    finalizeStep2(configurationID: number) {
        this.selectedConfiguration = configurationID;
        this.stepProgress = 3;
    }

    close() {
        this.$emit('close');
    }
}
