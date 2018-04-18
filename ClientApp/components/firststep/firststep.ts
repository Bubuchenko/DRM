import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';



@Component
export default class TaskFirstStepComponent extends Vue {
    $route: any;
    $http: any;


    @Prop({ default: false })
    value!: boolean;

    reset() {
        this.inProgress = false;
        this.showError = false;
        this.errorMessage = "";
        this.Name = "";
        this.Description = "";
        this.canProceed = false;
    }

    proceed() {
        this.$emit('proceed', this.basicParameters);
    }

    close() {
        this.$emit('close');
    }

    get basicParameters() {
        return {
            name: this.Name,
            description: this.Description
        };
    }

    inProgress: boolean = false;
    showError: boolean = false;
    errorMessage: any = "";
    Name: string = "";
    Description: string = "";
    canProceed: boolean = false;

    validateFirstStep() {
        this.inProgress = true;

        this.$http.post('Task/ValidateBasics', {
            Name: this.Name,
            Description: this.Description
        }).then((response: any) => {
            this.inProgress = false;
            this.canProceed = true;
        }).catch((error: any) => {
            this.inProgress = false;
            this.errorMessage = error.response.data;
            this.showError = true;
            this.canProceed = false;
        });
    }
}
