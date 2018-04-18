import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';



@Component
export default class TaskFourthStepComponent extends Vue {
    $route: any;
    $http: any;

    configuration: any = {};
    username: string = "";
    userHasAgreedWithTerms: boolean = false;
    inProgress: boolean = false;

    @Prop()
    value!: any;

    reset() {
        this.configuration = {};
        this.username = "";
        this.userHasAgreedWithTerms = false;
        this.value = {};
    }

    previous() {
        this.$emit('previous');
    }

    proceed() {
        this.$emit('proceed');
        this.inProgress = true;
    }

    close() {
        this.$emit('close');
    }

    get actionString() {
        this.getConfiguration();
        this.getUsername();

        var result = "When this task is executed it will";

        if (this.value.setup.type == "REMOVE")
            result += " <kbd>remove all records</kbd>";
        else if (this.value.setup.type == "NULL")
            result += " <kbd>null out the values</kbd>";
        else
            result += ` <kbd>${this.value.setup.type} HASH</kbd> the values `;

        if (this.value.setup.type != "REMOVE")
            result += ` from column <kbd>${this.value.setup.columnName}</kbd>`;


        result += ` in table <kbd>${this.value.setup.tableName}</kbd> from database <kbd>${this.configuration.database}</kbd> residing on server <kbd>${this.configuration.server}</kbd>`;

        result += `, where the date in column <kbd>${this.value.setup.filterColumn}</kbd> is older than <kbd>${this.value.setup.periodInMonths} months</kbd> from the date on which the task is being executed.`;

        result += ` This results of this <kbd>CAN NOT BE UNDONE</kbd>. The publisher of this task, <kbd>${this.username}</kbd> is entirely responsible for any loss of data, barring any bugs or technical flaws in Data Retention Manager itself.`

        result+= ` This task will not be executed until it has been scheduled and can still be modified and deleted now or after creation.`

        return result;
    }

    getConfiguration() {
        this.$http.get('Configuration/Get', {
            params: {
                id: this.value.configurationID
            }
        }).then((response: any) => {
            this.configuration = response.data;
        }).catch((error: any) => {
            alert(error);
        });
    }

    getUsername() {
        this.$http.get('Home/Username', {
        }).then((response: any) => {
            this.username = response.data;
        }).catch((error: any) => {
            alert(error);
        });
    }
}
