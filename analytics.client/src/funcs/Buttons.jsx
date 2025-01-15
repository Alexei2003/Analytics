import './Buttons.css';
import { logOut, getUserRole, getEmployeeId } from './CookiesManager';

export function AddButtonListEmployees(navigate) {
    return(
        <button type="button" onClick={() => navigateListEmployees()} className="menu-btn animate-bounce">Список работников</button>
    )

    async function navigateListEmployees() {
        navigate(`/listemployees`);
    }

}
export function AddButtonUserProfile(navigate) {
    const tokenUserId = getEmployeeId();
    return (
        <button type="button" onClick={() => navigateToEmployee(tokenUserId)} className="profile-btn animate-bounce">Профиль пользователя</button>
    )

    async function navigateToEmployee(id) {
        navigate(`/employee/${id}`);
    }
}
export function AddButtonListSurveysWithId(navigate, id) {
    const tokenUserId = getEmployeeId();
    if (tokenUserId === id) {
        return (
            <button type="button" onClick={() => navigateListSurveys()} className="list-surveys-btn animate-bounce">Список опросов</button>
        );
    }

    return null;

    async function navigateListSurveys() {
        navigate(`/listsurveys/${-1}`);
    }
}
export function AddButtonAnalytics(navigate) {
    const role = getUserRole();
    if (role === 'HR' || role === 'Admin') {
        return (
            <button type="button" onClick={() => navigateAnalytics()} className="analytics-btn animate-bounce">Аналитика по работникам</button>
        );
    }

    return null;

    async function navigateAnalytics() {
        navigate(`/analytics`);
    }
}

export function AddButtonSurveyCreator(navigate) {
    const role = getUserRole();
    if (role === 'HR') {
        return (
            <button type="button" onClick={() => navigateSurveyCreator()} className="survey-creator-btn animate-bounce">Добавление опроса</button>
        );
    }

    return null;

    async function navigateSurveyCreator() {
        navigate(`/surveycreator`);
    }
}

export function AddButtonLogOutWithId(navigate, id) {
    const tokenUserId = getEmployeeId();
    if (tokenUserId === id) {
        return (
            <button type="button" onClick={() => { logOut(navigate);}} className="logout-btn animate-bounce">Выход</button>
        );
    }

    return null;
}

export function AddButtonAdminPanel(navigate) {
    const role = getUserRole();
    if (role === 'Admin') {
        return (
            <button type="button" onClick={() => navigateAdminPanel()} className="panel-btn animate-bounce">Кабинет админа</button>
        );
    }

    return null;

    async function navigateAdminPanel() {
        navigate(`/adminpanel`);
    }
}
export function AddButtonHRPanel(navigate) {
    const role = getUserRole();
    if (role === 'HR') {
        return (
            <button type="button" onClick={() => navigateHRPanel()} className="panel-btn animate-bounce">Кабинет HR</button>
        );
    }

    return null;

    async function navigateHRPanel() {
        navigate(`/hrpanel`);
    }
}

export function AddButtonAnalyticsAllUserSurvey(navigate) {
    const role = getUserRole();
    if (role === 'HR' || role === 'Admin') {
        return (
            <button type="button" onClick={() => navigateAnalyticsAllUserSurvey()} className="analytics-btn animate-bounce">Аналитика по опросам</button>
        );
    }

    return null;

    async function navigateAnalyticsAllUserSurvey() {
        navigate(`/listsurveys/${0}`);
    }
}

export function AddButtonAnalyticsUserSurvey(navigate, employeeId) {
    const role = getUserRole();
    if (role === 'HR' || role === 'Admin') {
        return (
            <button type="button" onClick={() => navigateAnalyticsAllUserSurvey()} className="analytics-btn animate-bounce">Аналитика по опросам</button>
        );
    }

    return null;

    async function navigateAnalyticsAllUserSurvey() {
        navigate(`/listsurveys/${employeeId}`);
    }
}

export function AddButtonListReportsUser(navigate, employeeId) {
    const role = getUserRole();
    if (role === 'HR' || role === 'Admin') {
        return (
            <button type="button" onClick={() => navigateListReports()} className="analytics-btn animate-bounce">Список отчетов</button>
        );
    }

    return null;

    async function navigateListReports() {
        navigate(`/listreports/${employeeId}`);
    }
}
export function AddButtonListReports(navigate) {
    const role = getUserRole();
    if (role === 'HR' || role === 'Admin') {
        return (
            <button type="button" onClick={() => navigateListReports()} className="analytics-btn animate-bounce">Список отчетов</button>
        );
    }

    return null;

    async function navigateListReports() {
        navigate(`/listreports/${0}`);
    }
}

export function AddButtonAddReport(navigate, id) {
    const role = getUserRole();
    if (role === 'HR') {
        return (
            <button type="button" onClick={() => navigateReport()} className="survey-creator-btn animate-bounce"> Создать отчет</button>
        );
    }

    return null;

    async function navigateReport() {
        navigate(`/report/${0}/${id}`);
    }
}