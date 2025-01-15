export function getCookie(name) {
    const nameEQ = name + "=";
    const cookies = document.cookie.split(';');

    for (let i = 0; i < cookies.length; i++) {
        let cookie = cookies[i].trim();
        if (cookie.indexOf(nameEQ) === 0) {
            return cookie.substring(nameEQ.length, cookie.length); 
        }
    }
    return null; 
}

export const getUserRole = () => {
    return getCookie('UserRole');
}

export const getEmployeeId = () => {
    return getCookie('EmployeeId');
}

export const logOut = async (navigate) =>{
    try {
        const response = await fetch('authorization', {
            method: 'DELETE',
            headers: {
                "Content-type": "application/json"
            }
        })
        if (response.ok) {
            navigate('/');
        }

        if (response.status === 401) {
            console.error('Ошибка при получении данных:', response.statusText);
            navigate('/');
        }
    } catch (error) {
        console.error('Ошибка при отправке:', error);
    }
}
