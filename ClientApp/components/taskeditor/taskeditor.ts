import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';


@Component({
    components: {
        TaskFirstStepComponent: require('../firststep/firststep.vue.html'),
        TaskSecondStepComponent: require('../secondstep/secondstep.vue.html'),
        TaskThirdStepComponent: require('../thirdstep/thirdstep.vue.html'),
        TaskFourthStepComponent: require('../fourthstep/fourthstep.vue.html')
    }
})
export default class TaskEditorComponent extends Vue {
    $route: any;
    $http: any;
    stepProgress: any = 1;
    inProgress: boolean = false;

    @Prop({ default: false })
    value!: boolean;

    @Prop({ default: 0})
    applicationid!: number;

    taskParameters: any = {
        name: "",
        description: "",
        configurationID: 0,
        setup: {}
    }

    
    finalizeStep1(basicParameters: any) {
        this.taskParameters.name = basicParameters.name;
        this.taskParameters.description = basicParameters.description;
        this.stepProgress = 2;
    }

    finalizeStep2(configurationID: number) {
        this.taskParameters.configurationID = configurationID;
        this.stepProgress = 3;
    }

    finalizeStep3(setupObject: any) {
        this.taskParameters.setup = setupObject;
        this.stepProgress = 4;
    }

    finalizeStep4() {
        this.createTask();
    }

    close() {
        this.$emit('close');
    }

    createTask() {
        this.inProgress = true;

        this.$http.post('Task/CreateTask', {
            Name: this.taskParameters.name,
            Description: this.taskParameters.description,
            ConfigurationID: this.taskParameters.configurationID,
            ApplicationID: this.applicationid,
            TableName: this.taskParameters.setup.tableName,
            ColumnName: this.taskParameters.setup.columnName,
            Type: this.taskParameters.setup.type,
            FilterColumn: this.taskParameters.setup.filterColumn,
            periodInMonths: this.taskParameters.setup.periodInMonths

        }).then((response: any) => {
            this.$emit('created');
            this.close();
            this.inProgress = false;
        }).catch((error: any) => {
            this.inProgress = false;
            console.log(error);
        });
    }
}
